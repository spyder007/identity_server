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
  faSave,
  faShield,
} from "@fortawesome/free-solid-svg-icons";

import PageHeader from "../../components/PageHeader";
import {
  getApiV1IdentityResourcesById,
  postApiV1IdentityResources,
  putApiV1IdentityResourcesById,
} from "../../api/generated/sdk.gen";
import type {
  IdentityResourceDto,
  SaveIdentityResourceDto,
} from "../../api/generated/types.gen";
import { ClaimsPanel, PropertiesPanel } from "./IdentityResourceSubResources";

function makeEmptyDraft(): SaveIdentityResourceDto {
  return {
    name: "",
    displayName: "",
    enabled: true,
    emphasize: false,
    required: false,
    showInDiscoveryDocument: true,
    nonEditable: false,
  };
}

function toDraft(dto: IdentityResourceDto): SaveIdentityResourceDto {
  return {
    name: dto.name ?? "",
    displayName: dto.displayName ?? null,
    description: dto.description ?? null,
    enabled: dto.enabled ?? false,
    emphasize: dto.emphasize ?? false,
    required: dto.required ?? false,
    showInDiscoveryDocument: dto.showInDiscoveryDocument ?? false,
    nonEditable: dto.nonEditable ?? false,
  };
}

export default function IdentityResourceEdit() {
  const { id: idParam } = useParams<{ id: string }>();
  const navigate = useNavigate();
  const toast = useRef<Toast>(null);
  const isNew = !idParam;
  const numericId = idParam ? Number(idParam) : 0;

  const [draft, setDraft] = useState<SaveIdentityResourceDto>(makeEmptyDraft);
  const [loading, setLoading] = useState(!isNew);
  const [saving, setSaving] = useState(false);
  const [activeTab, setActiveTab] = useState(0);

  const load = useCallback(async () => {
    if (isNew) return;
    setLoading(true);
    const r = await getApiV1IdentityResourcesById({ path: { id: numericId } });
    if (!r.error && r.data) setDraft(toDraft(r.data));
    setLoading(false);
  }, [isNew, numericId]);

  useEffect(() => {
    void load();
  }, [load]);

  const update = <K extends keyof SaveIdentityResourceDto>(
    key: K,
    value: SaveIdentityResourceDto[K],
  ) => setDraft((d) => ({ ...d, [key]: value }));

  const submit = async (e: React.FormEvent) => {
    e.preventDefault();
    setSaving(true);
    try {
      if (isNew) {
        const r = await postApiV1IdentityResources({ body: draft });
        if (!r.error && r.data?.id !== undefined) {
          toast.current?.show({ severity: "success", summary: "Created", detail: "Identity resource created.", life: 3000 });
          navigate(`/identity-resources/${r.data.id}`, { replace: true });
        }
      } else {
        const r = await putApiV1IdentityResourcesById({ path: { id: numericId }, body: draft });
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
        icon={faShield}
        eyebrow={isNew ? "New" : "Identity resource"}
        title={isNew ? "Create identity resource" : draft.displayName || draft.name || "Edit identity resource"}
        subtitle={isNew ? "Define a claim group (scope) that clients can request" : `Name: ${draft.name}`}
        actions={
          <Link to="/identity-resources">
            <Button outlined label="Back" icon={<FontAwesomeIcon icon={faArrowLeft} />} />
          </Link>
        }
      />

      <form onSubmit={submit}>
        <TabView scrollable activeIndex={activeTab} onTabChange={(e) => setActiveTab(e.index)}>
          <TabPanel header="Settings">
            <div className="space-y-8">
              <Section
                title="Basic information"
                description="Identity and display details for this identity resource."
              >
                <FieldGrid>
                  <Field label="Name" required>
                    <InputText
                      value={draft.name}
                      onChange={(e) => update("name", e.target.value)}
                      required
                      disabled={!isNew}
                      placeholder="profile"
                    />
                  </Field>
                  <Field label="Display name">
                    <InputText
                      value={draft.displayName ?? ""}
                      onChange={(e) => update("displayName", e.target.value || null)}
                      placeholder="User profile"
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
                    description="Disabled identity resources cannot be requested."
                    checked={draft.enabled ?? false}
                    onChange={(v) => update("enabled", v)}
                  />
                </SwitchList>
              </Section>

              <Section
                title="Consent behaviour"
                description="How this resource appears on the consent screen."
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
                  <SwitchRow
                    label="Non-editable"
                    description="Mark as read-only to prevent further changes."
                    checked={draft.nonEditable ?? false}
                    onChange={(v) => update("nonEditable", v)}
                  />
                </SwitchList>
              </Section>
            </div>
          </TabPanel>

          {!isNew && <TabPanel header="User claims"><ClaimsPanel identityResourceId={numericId} /></TabPanel>}
          {!isNew && <TabPanel header="Properties"><PropertiesPanel identityResourceId={numericId} /></TabPanel>}
        </TabView>

        <div className="fixed inset-x-0 bottom-0 left-60 z-10 border-t border-border bg-surface/95 backdrop-blur">
          <div className="mx-auto flex w-full max-w-6xl items-center justify-end gap-2 px-6 py-3 md:px-8">
            <Link to="/identity-resources">
              <Button type="button" outlined label="Cancel" />
            </Link>
            <Button
              type="submit"
              loading={saving}
              label={isNew ? "Create identity resource" : "Save changes"}
              icon={<FontAwesomeIcon icon={faSave} />}
            />
          </div>
        </div>
      </form>
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
