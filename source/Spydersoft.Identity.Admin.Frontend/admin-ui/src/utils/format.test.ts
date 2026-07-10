import { describe, it, expect } from "vitest";
import { formatDate, toNumberOrUndefined, asId } from "./format";

describe("formatDate", () => {
  it("returns an empty string for null/undefined/empty input", () => {
    expect(formatDate(null)).toBe("");
    expect(formatDate(undefined)).toBe("");
    expect(formatDate("")).toBe("");
  });

  it("returns the original value when it is not a parseable date", () => {
    expect(formatDate("not-a-date")).toBe("not-a-date");
  });

  it("formats a valid ISO date into a human-readable string", () => {
    const result = formatDate("2024-03-15T00:00:00Z");
    expect(result).not.toBe("");
    // Locale-independent assertion: the year is always present.
    expect(result).toContain("2024");
  });
});

describe("toNumberOrUndefined", () => {
  it("returns undefined for null, undefined, and empty string", () => {
    expect(toNumberOrUndefined(null)).toBeUndefined();
    expect(toNumberOrUndefined(undefined)).toBeUndefined();
    expect(toNumberOrUndefined("")).toBeUndefined();
  });

  it("returns the number unchanged when given a number", () => {
    expect(toNumberOrUndefined(42)).toBe(42);
    expect(toNumberOrUndefined(0)).toBe(0);
  });

  it("parses numeric strings", () => {
    expect(toNumberOrUndefined("7")).toBe(7);
    expect(toNumberOrUndefined("3.5")).toBe(3.5);
  });

  it("returns undefined for non-numeric or non-finite values", () => {
    expect(toNumberOrUndefined("abc")).toBeUndefined();
    expect(toNumberOrUndefined(Infinity)).toBeUndefined();
    expect(toNumberOrUndefined(NaN)).toBeUndefined();
  });
});

describe("asId", () => {
  it("returns an empty string for undefined", () => {
    expect(asId(undefined)).toBe("");
  });

  it("stringifies numbers and passes strings through", () => {
    expect(asId(123)).toBe("123");
    expect(asId("abc")).toBe("abc");
    expect(asId(0)).toBe("0");
  });
});
