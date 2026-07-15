import { useCallback, useEffect, useRef, useState } from "react";
import { Link, useNavigate, useParams } from "react-router-dom";
import { InputText } from "primereact/inputtext";
import { InputTextarea } from "primereact/inputtextarea";
import { InputSwitch } from "primereact/inputswitch";
import { Button } from "primereact/button";
import { TabView, TabPanel } from "primereact/tabview";
import { Toast } from "primereact/toast";
import { FontAwesomeIcon } from "@fortawesome/react-fontawesome";
import {
  faArrowLeft,
  faKey,
  faSave,
} from "@fortawesome/free-solid-svg-icons";

import PageHeader from "../../components/PageHeader";
import {
  getApiV1ScopesById,
  postApiV1Scopes,
  putApiV1ScopesById,
} from "../../api/generated/sdk.gen";
import type { ScopeDto, SaveScopeDto } from "../../api/generated/types.gen";
import { ClaimsPanel, PropertiesPanel } from "./ScopeSubResources";

function makeEmptyDraft(): SaveScopeDto {
  return {
    name: "",
    displayName: "",
    enabled: true,
    emphasize: false,
    required: false,
    showInDiscoveryDocument: true,
  };
}

function toDraft(dto: ScopeDto): SaveScopeDto {
  return {
    name: dto.name ?? "",
    displayName: dto.displayName ?? null,
    description: dto.description ?? null,
    enabled: dto.enabled ?? false,
    emphasize: dto.emphasize ?? false,
    required: dto.required ?? false,
    showInDiscoveryDocument: dto.showInDiscoveryDocument ?? false,
  };
}

export default function ScopeEdit() {
  const { id: idParam } = useParams<{ id: string }>();
  const navigate = useNavigate();
  const toast = useRef<Toast>(null);
  const isNew = !idParam;
  const numericId = idParam ? Number(idParam) : 0;

  const [draft, setDraft] = useState<SaveScopeDto>(makeEmptyDraft);
  const [loading, setLoading] = useState(!isNew);
  const [saving, setSaving] = useState(false);
  const [activeTab, setActiveTab] = useState(0);

  const load = useCallback(async () => {
    if (isNew) return;
    setLoading(true);
    const r = await getApiV1ScopesById({ path: { id: numericId } });
    if (!r.error && r.data) setDraft(toDraft(r.data));
    setLoading(false);
  }, [isNew, numericId]);

  useEffect(() => {
    void load();
  }, [load]);

  const update = <K extends keyof SaveScopeDto>(key: K, value: SaveScopeDto[K]) =>
    setDraft((d) => ({ ...d, [key]: value }));

  const submit = async () => {
    setSaving(true);
    try {
      if (isNew) {
        const r = await postApiV1Scopes({ body: draft });
        if (!r.error && r.data?.id !== undefined) {
          toast.current?.show({ severity: "success", summary: "Created", detail: "Scope created.", life: 3000 });
          navigate(`/scopes/${r.data.id}`, { replace: true });
        }
      } else {
        const r = await putApiV1ScopesById({ path: { id: numericId }, body: draft });
        if (!r.error) {
          toast.current?.show({ severity: "success", summary: "Saved", detail: "Changes saved.", life: 3000 });
        }
      }
    } finally {
      setSaving(false);
    }
  };

  if (loading) {
    return <div className="p-4 text-content-muted">Loading…</div>;
  }

  return (
    <div className="pb-24">
      <Toast ref={toast} position="top-right" />
      <PageHeader
        icon={faKey}
        eyebrow={isNew ? "New" : "Scope"}
        title={isNew ? "Create scope" : draft.displayName || draft.name || "Edit scope"}
        subtitle={isNew ? "Define a permission clients can request" : `Name: ${draft.name}`}
        actions={
          <Link to="/scopes">
            <Button outlined label="Back" icon={<FontAwesomeIcon icon={faArrowLeft} />} />
          </Link>
        }
      />

      <TabView scrollable activeIndex={activeTab} onTabChange={(e) => setActiveTab(e.index)}>
          <TabPanel header="Settings">
            <div className="space-y-8">
              <Section
                title="Basic information"
                description="Identity and display details for this scope."
              >
                <FieldGrid>
                  <Field label="Name" required>
                    <InputText
                      value={draft.name}
                      onChange={(e) => update("name", e.target.value)}
                      required
                      disabled={!isNew}
                      placeholder="my.api.read"
                    />
                  </Field>
                  <Field label="Display name">
                    <InputText
                      value={draft.displayName ?? ""}
                      onChange={(e) => update("displayName", e.target.value || null)}
                      placeholder="Read access to My API"
                    />
                  </Field>
                  <Field label="Description" className="md:col-span-2">
                    <InputTextarea
                      rows={2}
                      value={draft.description ?? ""}
                      onChange={(e) => update("description", e.target.value || null)}
                    />
                  </Field>
                </FieldGrid>
                <SwitchList>
                  <SwitchRow
                    label="Enabled"
                    description="Disabled scopes cannot be requested."
                    checked={draft.enabled ?? false}
                    onChange={(v) => update("enabled", v)}
                  />
                </SwitchList>
              </Section>

              <Section
                title="Consent behaviour"
                description="How this scope appears on the consent screen and discovery document."
              >
                <SwitchList>
                  <SwitchRow
                    label="Required"
                    description="User cannot deconsent this scope on the consent screen."
                    checked={draft.required ?? false}
                    onChange={(v) => update("required", v)}
                  />
                  <SwitchRow
                    label="Emphasize"
                    description="Highlight this scope on the consent screen as sensitive."
                    checked={draft.emphasize ?? false}
                    onChange={(v) => update("emphasize", v)}
                  />
                  <SwitchRow
                    label="Show in discovery document"
                    description="Advertise this scope on the OIDC discovery endpoint."
                    checked={draft.showInDiscoveryDocument ?? false}
                    onChange={(v) => update("showInDiscoveryDocument", v)}
                  />
                </SwitchList>
              </Section>
            </div>
          </TabPanel>

          {!isNew && <TabPanel header="User claims"><ClaimsPanel scopeId={numericId} /></TabPanel>}
          {!isNew && <TabPanel header="Properties"><PropertiesPanel scopeId={numericId} /></TabPanel>}
        </TabView>

        <div className="fixed inset-x-0 bottom-0 left-60 z-10 border-t border-border bg-surface/95 backdrop-blur">
          <div className="mx-auto flex w-full max-w-6xl items-center justify-end gap-2 px-6 py-3 md:px-8">
            <Link to="/scopes">
              <Button type="button" outlined label="Cancel" />
            </Link>
            <Button
              type="button"
              onClick={submit}
              loading={saving}
              label={isNew ? "Create scope" : "Save changes"}
              icon={<FontAwesomeIcon icon={faSave} />}
            />
          </div>
        </div>
    </div>
  );
}

function Section({
  title,
  description,
  children,
}: Readonly<{
  title: string;
  description?: string;
  children: React.ReactNode;
}>) {
  return (
    <section className="rounded-xl border border-border bg-surface p-5 shadow-sm md:p-6">
      <header className="mb-5">
        <h2 className="text-base font-semibold text-content">{title}</h2>
        {description && (
          <p className="mt-0.5 text-sm text-content-muted">{description}</p>
        )}
      </header>
      <div className="space-y-5">{children}</div>
    </section>
  );
}

function FieldGrid({ children }: Readonly<{ children: React.ReactNode }>) {
  return <div className="grid grid-cols-1 gap-4 md:grid-cols-2">{children}</div>;
}

function SwitchList({ children }: Readonly<{ children: React.ReactNode }>) {
  return (
    <div className="divide-y divide-border rounded-md border border-border">
      {children}
    </div>
  );
}

function Field({
  label,
  required,
  className,
  children,
}: Readonly<{
  label: string;
  required?: boolean;
  className?: string;
  children: React.ReactNode;
}>) {
  return (
    <div className={className ?? ""}>
      <label className="mb-1.5 block text-sm font-medium text-content">
        {label}
        {required && <span className="ml-1 text-danger">*</span>}
      </label>
      <div className="w-full [&_.p-inputtext]:w-full [&_.p-inputtextarea]:w-full [&_.p-inputnumber]:w-full">
        {children}
      </div>
    </div>
  );
}

function SwitchRow({
  label,
  description,
  checked,
  onChange,
}: Readonly<{
  label: string;
  description?: string;
  checked: boolean;
  onChange: (next: boolean) => void;
}>) {
  return (
    <label className="flex cursor-pointer items-center justify-between gap-4 px-4 py-3 transition-colors hover:bg-surface-muted">
      <span className="flex min-w-0 flex-1 items-start gap-2">
        <span className="min-w-0">
          <span className="block text-sm font-medium text-content">{label}</span>
          {description && (
            <span className="mt-0.5 block text-xs text-content-muted">{description}</span>
          )}
        </span>
      </span>
      <InputSwitch checked={checked} onChange={(e) => onChange(Boolean(e.value))} />
    </label>
  );
}
