import os
from dotenv import load_dotenv
import openai
from fastapi import FastAPI
from fastapi.middleware.cors import CORSMiddleware
from pydantic import BaseModel

load_dotenv()

YANDEX_API_KEY = os.getenv("YANDEX_API_KEY")
YANDEX_FOLDER_ID = os.getenv("YANDEX_FOLDER_ID")
YANDEX_MODEL = os.getenv("YANDEX_MODEL", "yandexgpt-lite")

if not YANDEX_API_KEY:
    raise ValueError("YANDEX_API_KEY not set")
if not YANDEX_FOLDER_ID:
    raise ValueError("YANDEX_FOLDER_ID not set")

SYSTEM_PROMPT = """
Ты - ассистент-ресерчер, который ищет в интернете актуальные материалы
и полезные ресурсы.
Ты специализируешься на поиске свежих статей, новостей, исследований и
инструментов.
Правила:
1. Всегда сначала используй интернет-поиск.
2. Отдавай приоритет свежим материалам.
3. Отдавай приоритет авторитетным источникам: официальные сайты,
   научные публикации, GitHub, документация.
4. Не придумывай ссылки - используй только найденные материалы.
5. В ответе всегда давай ровно 5 ссылок.
6. Для каждой ссылки укажи:
   - название материала;
   - URL;
   - 1–2 предложения, почему источник полезен.
Если прямых материалов мало, расширяй поиск на англоязычные источники, но
всё равно дай 5 ссылок.
Сначала дай короткий вывод (1–2 предложения), затем список ссылок.
Важно: всегда отвечай только на русском языке.
Даже если источники на английском - переводи заголовки и описания на
русский;
Не используй английский язык в тексте ответа, кроме оригинальных
названий продуктов, компаний и ссылок;
Не переключайся на английский ни при каких условиях.
"""

client = openai.OpenAI(
    api_key=YANDEX_API_KEY,
    base_url="https://ai.api.cloud.yandex.net/v1",
    project=YANDEX_FOLDER_ID,
)

def research(query: str) -> str:
    # Исправлено: web_reserch -> web_search
    response = client.responses.create(
        model=f"gpt://{YANDEX_FOLDER_ID}/{YANDEX_MODEL}",
        instructions=SYSTEM_PROMPT,
        input=query,
        tools=[
            {
                "type": "web_search",
            }
        ],
        temperature=0.2,
        max_output_tokens=1800,
    )

    text_parts: list[str] = []

    if hasattr(response, "output") and response.output:
        for item in response.output:
            if hasattr(item, "content") and item.content:
                for content_item in item.content:
                    text = getattr(content_item, "text", None)
                    if text:
                        text_parts.append(text)

    return "\n".join(text_parts).strip()

app = FastAPI(title="ILP Yandex Research Service")

# 1. Настройка CORS — РАЗРЕШАЕМ ЗАПРОСЫ С ВАШЕГО ФРОНТЕНДА
app.add_middleware(
    CORSMiddleware,
    allow_origins=[
        "https://smurw632.github.io",
        "https://alicagpt-qjgz.onrender.com",  # Сам сервис
        "http://localhost:3001",               # Локальная разработка
        "http://localhost:5004",               # .NET бэкенд
        "http://localhost:54114",              # Vue локально
    ],
    allow_credentials=True,
    allow_methods=["*"],
    allow_headers=["*"],
)

# 2. Health check endpoints (для Render)
@app.get("/health")
async def health_check():
    return {"status": "healthy", "service": "Yandex AI"}

@app.get("/ping")
async def ping():
    return {"status": "ok"}

# 3. Основной эндпоинт для поиска
class ResearchRequest(BaseModel):
    query: str

class ResearchResponse(BaseModel):
    result: str

@app.post("/research")
async def research_endpoint(req: ResearchRequest):
    try:
        result = research(req.query)
        return ResearchResponse(result=result)
    except Exception as e:
        return ResearchResponse(result=f"Ошибка: {str(e)}")

# Дополнительный эндпоинт для совместимости с фронтендом
@app.post("/research/search")
async def research_search(req: ResearchRequest):
    return await research_endpoint(req)
