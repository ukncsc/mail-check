export class StringUtils {
  public static isUndefinedNullOrWhitespace(str: string): boolean {
    if (
      typeof str == 'undefined' ||
      !str ||
      str.length === 0 ||
      str === '' ||
      !/[^\s]/.test(str) ||
      /^\s*$/.test(str) ||
      str.replace(/\s/g, '') === ''
    ) {
      return true;
    } else {
      return false;
    }
  }
}
