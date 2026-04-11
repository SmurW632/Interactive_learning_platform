import os
from dotenv import load_dotenv
import openai
from fastapi import FastAPI
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
    response = client.responses.create(
        model=f"gpt://{YANDEX_FOLDER_ID}/{YANDEX_MODEL}",
        instructions=SYSTEM_PROMPT,
        input=query,
        tools=[
            {
                "type": "web_reserch",
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

class ResearchRequest(BaseModel):
    query: str

class ResearchResponse(BaseModel):
    result: str

@app.post("/research", response_model=ResearchResponse)
def research_endpoint(req: ResearchRequest):
    result = research(req.query)
    return ResearchResponse(result=result)
