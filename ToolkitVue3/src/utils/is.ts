const toString = Object.prototype.toString;

export function is(val: unknown, type: string) {
  return toString.call(val) === `[object ${type}]`;
}
export function isDef<T = unknown>(val?: T): val is T {
  return typeof val !== 'undefined';
}
export function isNull(val: unknown): val is null {
  return val === null;
}
export function isUnDef<T = unknown>(val?: T): val is T {
  return !isDef(val);
}
export function isString(val: unknown): val is string {
  return is(val, 'String');
}

export function isNullAndUnDef(val: unknown): val is null | undefined {
  return isUnDef(val) && isNull(val);
}