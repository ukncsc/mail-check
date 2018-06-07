import { domainRegex } from './AdminHelpers';

describe('DomainRegex', () => {
  test('should not match only tld with .', () =>
    expect(domainRegex.test('.ab')).toBe(false));

  test('should not match null', () =>
    expect(domainRegex.test(null)).toBe(false));

  test('should not match empty string', () =>
    expect(domainRegex.test('')).toBe(false));

  test('should not match only tld', () =>
    expect(domainRegex.test('ab')).toBe(false));

  test('should not match only tld 1 char long', () =>
    expect(domainRegex.test('a.b')).toBe(false));

  test('should not match tld with numbers', () =>
    expect(domainRegex.test('a.11')).toBe(false));

  test('should not match element starting with hyphen', () =>
    expect(domainRegex.test('12.-12.ab')).toBe(false));

  test('should not match element containing dollar', () =>
    expect(domainRegex.test('a$.ab')).toBe(false));

  test('should not match element containing @', () =>
    expect(domainRegex.test('a@.ab')).toBe(false));

  test('should not match element containing ampersand', () =>
    expect(domainRegex.test('a&.ab')).toBe(false));

  test('should not match element containing colon', () =>
    expect(domainRegex.test('a:.ab')).toBe(false));

  test('should not match element containing semi colon', () =>
    expect(domainRegex.test('a;.ab')).toBe(false));

  test('should not match subdomain with 64 chars', () =>
    expect(
      domainRegex.test(
        'aaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaa.ab'
      )
    ).toBe(false));

  test('should not match tld with 64 chars', () =>
    expect(
      domainRegex.test(
        'a.aaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaa'
      )
    ).toBe(false));

  test('should not match length of 254', () =>
    expect(
      domainRegex.test(
        'aaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaa.aaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaa.aaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaa.aaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaa'
      )
    ).toBe(false));

  test('should match tld 2 chars long', () =>
    expect(domainRegex.test('a.ab')).toBe(true));

  test('should match multiple sub domains', () =>
    expect(domainRegex.test('a.c.d.e.f.ab')).toBe(true));

  test('should match element with numbers', () =>
    expect(domainRegex.test('1.ab')).toBe(true));

  test('should match element with www', () =>
    expect(domainRegex.test('www.ab')).toBe(true));

  test('should match element ending with hyphen', () =>
    expect(domainRegex.test('12.ab-.ab')).toBe(true));

  test('should match element not starting with hyphen', () =>
    expect(domainRegex.test('12.a-b.ab')).toBe(true));

  test('should match subdomain with 63 chars', () =>
    expect(
      domainRegex.test(
        'aaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaa.ab'
      )
    ).toBe(true));

  test('should match tld with 63 chars', () =>
    expect(
      domainRegex.test(
        'a.aaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaa'
      )
    ).toBe(true));

  test('should match length of 253', () =>
    expect(
      domainRegex.test(
        'aaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaa.aaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaa.aaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaa.aaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaa'
      )
    ).toBe(true));
});
