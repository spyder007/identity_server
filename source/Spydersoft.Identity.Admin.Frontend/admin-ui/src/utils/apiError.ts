// Extracts a human-readable message from an ASP.NET ProblemDetails/ValidationProblemDetails
// error body so failed API calls can be surfaced instead of failing silently.
export function problemMessage(error: unknown, fallback: string): string {
  if (error && typeof error === "object") {
    const e = error as { detail?: string; title?: string; errors?: Record<string, string[]> };
    const firstValidationError = e.errors && Object.values(e.errors)[0]?.[0];
    if (firstValidationError) return firstValidationError;
    if (e.detail) return e.detail;
    if (e.title) return e.title;
  }
  return fallback;
}
