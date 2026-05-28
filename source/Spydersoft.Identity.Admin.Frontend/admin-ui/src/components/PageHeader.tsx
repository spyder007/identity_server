import type { ReactNode } from "react";
import { FontAwesomeIcon, type FontAwesomeIconProps } from "@fortawesome/react-fontawesome";

interface PageHeaderProps {
  title: ReactNode;
  subtitle?: ReactNode;
  eyebrow?: ReactNode;
  icon?: FontAwesomeIconProps["icon"];
  actions?: ReactNode;
}

export default function PageHeader({ title, subtitle, eyebrow, icon, actions }: Readonly<PageHeaderProps>) {
  return (
    <div className="mb-6 flex flex-col gap-4 border-b border-border pb-5 sm:flex-row sm:items-end sm:justify-between">
      <div className="flex min-w-0 items-center gap-3.5">
        {icon && (
          <span className="flex h-10 w-10 shrink-0 items-center justify-center rounded-lg bg-brand-soft text-brand">
            <FontAwesomeIcon icon={icon} />
          </span>
        )}
        <div className="min-w-0">
          {eyebrow && (
            <div className="mb-0.5 text-[0.7rem] font-semibold uppercase tracking-wider text-content-subtle">
              {eyebrow}
            </div>
          )}
          <h1 className="truncate text-[1.5rem] font-semibold leading-tight text-content">
            {title}
          </h1>
          {subtitle && (
            <p className="mt-0.5 text-sm text-content-muted">{subtitle}</p>
          )}
        </div>
      </div>
      {actions && <div className="flex shrink-0 items-center gap-2">{actions}</div>}
    </div>
  );
}
