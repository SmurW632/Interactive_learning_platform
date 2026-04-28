import { describe, it, expect } from 'vitest'

describe('Basic Math Tests', () => {
  it('adds numbers correctly', () => {
    expect(1 + 1).toBe(2)
  })

  it('subtracts numbers correctly', () => {
    expect(5 - 3).toBe(2)
  })

  it('multiplies numbers correctly', () => {
    expect(3 * 4).toBe(12)
  })

  it('divides numbers correctly', () => {
    expect(10 / 2).toBe(5)
  })
})

describe('String Tests', () => {
  it('converts to uppercase', () => {
    expect('hello'.toUpperCase()).toBe('HELLO')
  })

  it('converts to lowercase', () => {
    expect('WORLD'.toLowerCase()).toBe('world')
  })

  it('trims whitespace', () => {
    expect('  test  '.trim()).toBe('test')
  })

  it('checks string length', () => {
    expect('hello'.length).toBe(5)
  })
})

describe('Array Tests', () => {
  it('checks array length', () => {
    expect([1, 2, 3].length).toBe(3)
  })

  it('checks array includes element', () => {
    expect([1, 2, 3]).toContain(2)
  })

  it('checks array map', () => {
    expect([1, 2, 3].map(x => x * 2)).toEqual([2, 4, 6])
  })
})

describe('Object Tests', () => {
  it('checks object properties', () => {
    const obj = { name: 'test', value: 42 }
    expect(obj).toHaveProperty('name')
    expect(obj.name).toBe('test')
  })

  it('checks object equality', () => {
    expect({ a: 1, b: 2 }).toEqual({ a: 1, b: 2 })
  })
})
