import { FontAwesomeIcon } from "@fortawesome/react-fontawesome";
import { faArrowRight, faShieldHalved } from "@fortawesome/free-solid-svg-icons";
import { Button } from "primereact/button";
import { useAuth } from "../context";

export default function SignInPage() {
  const { login, isLoading } = useAuth();

  return (
    <div className="relative flex min-h-screen items-center justify-center overflow-hidden bg-surface-muted p-6">
      <div
        aria-hidden
        className="pointer-events-none absolute inset-0 opacity-60"
        style={{
          background:
            "radial-gradient(60rem 30rem at 10% -10%, var(--color-brand-soft) 0%, transparent 60%), radial-gradient(40rem 20rem at 110% 110%, var(--color-brand-soft) 0%, transparent 60%)",
        }}
      />

      <div className="relative w-full max-w-sm">
        <div className="rounded-xl border border-border bg-surface p-7 shadow-sm">
          <div className="mb-6 flex items-center gap-3">
            <span className="flex h-11 w-11 items-center justify-center rounded-xl bg-brand text-brand-fg shadow-sm">
              <FontAwesomeIcon icon={faShieldHalved} className="text-lg" />
            </span>
            <div>
              <div className="text-base font-semibold text-content">Identity Admin</div>
              <div className="text-xs text-content-muted">Spydersoft Identity Server</div>
            </div>
          </div>

          <h1 className="mb-1 text-xl font-semibold text-content">Sign in to continue</h1>
          <p className="mb-6 text-sm text-content-muted">
            You'll be redirected to your identity provider to authenticate.
          </p>

          <Button
            label={isLoading ? "Checking session…" : "Sign in"}
            icon={<FontAwesomeIcon icon={faArrowRight} />}
            iconPos="right"
            onClick={login}
            disabled={isLoading}
            className="w-full"
          />
        </div>

        <p className="mt-4 text-center text-xs text-content-subtle">
          Need access? Contact your administrator.
        </p>
      </div>
    </div>
  );
}
