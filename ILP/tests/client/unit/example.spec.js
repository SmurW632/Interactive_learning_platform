import { describe, it, expect } from 'vitest'

describe('Basic Math', () => {
  it('adds numbers correctly', () => {
    expect(1 + 1).toBe(2)
  })

  it('subtracts numbers correctly', () => {
    expect(5 - 3).toBe(2)
  })

  it('multiplies numbers correctly', () => {
    expect(3 * 4).toBe(12)
  })
})

describe('String Utils', () => {
  it('converts to uppercase', () => {
    expect('hello'.toUpperCase()).toBe('HELLO')
  })

  it('trims whitespace', () => {
    expect('  test  '.trim()).toBe('test')
  })

  it('fails intentionally', () => {
  expect(true).toBe(false)
})
})
