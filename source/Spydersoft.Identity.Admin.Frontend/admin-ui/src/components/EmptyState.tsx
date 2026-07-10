import type { ReactNode } from "react";
import { FontAwesomeIcon, type FontAwesomeIconProps } from "@fortawesome/react-fontawesome";

interface EmptyStateProps {
  icon?: FontAwesomeIconProps["icon"];
  title: ReactNode;
  description?: ReactNode;
  action?: ReactNode;
  /** When true, render compact (used inline inside a card). Default is a roomy full-page state. */
  compact?: boolean;
}

export default function EmptyState({
  icon,
  title,
  description,
  action,
  compact,
}: Readonly<EmptyStateProps>) {
  return (
    <div
      className={[
        "flex flex-col items-center justify-center text-center",
        compact
          ? "py-10"
          : "rounded-xl border border-border bg-surface px-6 py-16 shadow-sm",
      ].join(" ")}
    >
      {icon && (
        <span className="mb-4 flex h-14 w-14 items-center justify-center rounded-full bg-brand-soft text-xl text-brand">
          <FontAwesomeIcon icon={icon} />
        </span>
      )}
      <h2 className="text-lg font-semibold text-content">{title}</h2>
      {description && (
        <p className="mt-1.5 max-w-md text-sm text-content-muted">{description}</p>
      )}
      {action && <div className="mt-5">{action}</div>}
    </div>
  );
}
