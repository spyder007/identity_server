import { useCallback, useEffect, useRef, useState } from "react";
import { Link, useNavigate, useParams } from "react-router-dom";
import { InputText } from "primereact/inputtext";
import { InputNumber } from "primereact/inputnumber";
import { InputTextarea } from "primereact/inputtextarea";
import { InputSwitch } from "primereact/inputswitch";
import { Button } from "primereact/button";
import { TabView, TabPanel } from "primereact/tabview";
import { Toast } from "primereact/toast";
import { FontAwesomeIcon } from "@fortawesome/react-fontawesome";
import {
  faArrowLeft,
  faIdBadge,
  faSave,
  faTriangleExclamation,
} from "@fortawesome/free-solid-svg-icons";

import PageHeader from "../../components/PageHeader";
import {
  getApiV1ClientsById,
  postApiV1Clients,
  putApiV1ClientsById,
} from "../../api/generated/sdk.gen";
import type { ClientDto, SaveClientDto } from "../../api/generated/types.gen";
import {
  ClaimsPanel,
  CorsOriginsPanel,
  GrantTypesPanel,
  IdpRestrictionsPanel,
  PostLogoutRedirectUrisPanel,
  PropertiesPanel,
  RedirectUrisPanel,
  ScopesPanel,
  SecretsPanel,
} from "./ClientSubResources";

function makeEmptyDraft(): SaveClientDto {
  return {
    clientId: "",
    clientName: "",
    protocolType: "oidc",
    enabled: true,
    requireClientSecret: true,
    requireConsent: true,
    allowRememberConsent: true,
    requirePkce: true,
    accessTokenLifetime: 3600,
    identityTokenLifetime: 300,
    authorizationCodeLifetime: 300,
    deviceCodeLifetime: 300,
  };
}

function toDraft(dto: ClientDto): SaveClientDto {
  return {
    clientId: dto.clientId ?? "",
    clientName: dto.clientName ?? "",
    description: dto.description ?? null,
    clientUri: dto.clientUri ?? null,
    logoUri: dto.logoUri ?? null,
    requireClientSecret: dto.requireClientSecret ?? false,
    requireConsent: dto.requireConsent ?? false,
    requirePkce: dto.requirePkce ?? false,
    allowPlainTextPkce: dto.allowPlainTextPkce ?? false,
    allowOfflineAccess: dto.allowOfflineAccess ?? false,
    allowAccessTokensViaBrowser: dto.allowAccessTokensViaBrowser ?? false,
    allowRememberConsent: dto.allowRememberConsent ?? false,
    alwaysSendClientClaims: dto.alwaysSendClientClaims ?? false,
    alwaysIncludeUserClaimsInIdToken: dto.alwaysIncludeUserClaimsInIdToken ?? false,
    enableLocalLogin: dto.enableLocalLogin ?? true,
    includeJwtId: dto.includeJwtId ?? false,
    nonEditable: dto.nonEditable ?? false,
    updateAccessTokenClaimsOnRefresh: dto.updateAccessTokenClaimsOnRefresh ?? false,
    backChannelLogoutSessionRequired: dto.backChannelLogoutSessionRequired ?? false,
    frontChannelLogoutSessionRequired: dto.frontChannelLogoutSessionRequired ?? false,
    enabled: dto.enabled ?? false,
    absoluteRefreshTokenLifetime: dto.absoluteRefreshTokenLifetime ?? 0,
    accessTokenLifetime: dto.accessTokenLifetime ?? 0,
    accessTokenType: dto.accessTokenType ?? 0,
    authorizationCodeLifetime: dto.authorizationCodeLifetime ?? 0,
    deviceCodeLifetime: dto.deviceCodeLifetime ?? 0,
    identityTokenLifetime: dto.identityTokenLifetime ?? 0,
    refreshTokenExpiration: dto.refreshTokenExpiration ?? 0,
    refreshTokenUsage: dto.refreshTokenUsage ?? 0,
    slidingRefreshTokenLifetime: dto.slidingRefreshTokenLifetime ?? 0,
    consentLifetime: dto.consentLifetime ?? null,
    userSsoLifetime: dto.userSsoLifetime ?? null,
    backChannelLogoutUri: dto.backChannelLogoutUri ?? null,
    clientClaimsPrefix: dto.clientClaimsPrefix ?? null,
    frontChannelLogoutUri: dto.frontChannelLogoutUri ?? null,
    pairWiseSubjectSalt: dto.pairWiseSubjectSalt ?? null,
    protocolType: dto.protocolType ?? "oidc",
    userCodeType: dto.userCodeType ?? null,
  };
}

export default function ClientEdit() {
  const { id: idParam } = useParams<{ id: string }>();
  const navigate = useNavigate();
  const toast = useRef<Toast>(null);
  const isNew = !idParam;
  const numericId = idParam ? Number(idParam) : 0;

  const [draft, setDraft] = useState<SaveClientDto>(makeEmptyDraft);
  const [loading, setLoading] = useState(!isNew);
  const [saving, setSaving] = useState(false);
  const [activeTab, setActiveTab] = useState(0);

  const load = useCallback(async () => {
    if (isNew) return;
    setLoading(true);
    const r = await getApiV1ClientsById({ path: { id: numericId } });
    if (!r.error && r.data) setDraft(toDraft(r.data));
    setLoading(false);
  }, [isNew, numericId]);

  useEffect(() => {
    void load();
  }, [load]);

  const update = <K extends keyof SaveClientDto>(key: K, value: SaveClientDto[K]) =>
    setDraft((d) => ({ ...d, [key]: value }));

  const submit = async () => {
    setSaving(true);
    try {
      if (isNew) {
        const r = await postApiV1Clients({ body: draft });
        if (!r.error && r.data?.id !== undefined) {
          toast.current?.show({ severity: "success", summary: "Created", detail: "Client created.", life: 3000 });
          navigate(`/clients/${r.data.id}`, { replace: true });
        }
      } else {
        const r = await putApiV1ClientsById({ path: { id: numericId }, body: draft });
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
        icon={faIdBadge}
        eyebrow={isNew ? "New" : "Client"}
        title={isNew ? "Create client" : draft.clientName || draft.clientId || "Edit client"}
        subtitle={isNew ? "Register a new OAuth 2.0 / OIDC client" : `Client ID: ${draft.clientId}`}
        actions={
          <Link to="/clients">
            <Button outlined label="Back" icon={<FontAwesomeIcon icon={faArrowLeft} />} />
          </Link>
        }
      />

      <TabView scrollable activeIndex={activeTab} onTabChange={(e) => setActiveTab(e.index)}>
        <TabPanel header="Settings">
          <div className="space-y-8">
              <Section
                title="Basic information"
                description="Identity and display details for this client."
              >
                <FieldGrid>
                  <Field label="Client ID" required>
                    <InputText
                      value={draft.clientId}
                      onChange={(e) => update("clientId", e.target.value)}
                      required
                      disabled={!isNew}
                      placeholder="unique-client-id"
                    />
                  </Field>
                  <Field label="Display name" required>
                    <InputText
                      value={draft.clientName}
                      onChange={(e) => update("clientName", e.target.value)}
                      required
                      placeholder="My Application"
                    />
                  </Field>
                  <Field label="Client URI">
                    <InputText
                      type="url"
                      value={draft.clientUri ?? ""}
                      onChange={(e) => update("clientUri", e.target.value || null)}
                      placeholder="https://example.com"
                    />
                  </Field>
                  <Field label="Logo URI">
                    <InputText
                      type="url"
                      value={draft.logoUri ?? ""}
                      onChange={(e) => update("logoUri", e.target.value || null)}
                      placeholder="https://example.com/logo.png"
                    />
                  </Field>
                  <Field label="Description" className="md:col-span-2">
                    <InputTextarea
                      rows={2}
                      value={draft.description ?? ""}
                      onChange={(e) => update("description", e.target.value || null)}
                    />
                  </Field>
                  <Field label="Protocol type">
                    <InputText
                      value={draft.protocolType}
                      onChange={(e) => update("protocolType", e.target.value)}
                    />
                  </Field>
                </FieldGrid>
                <SwitchList>
                  <SwitchRow
                    label="Enabled"
                    description="Disabled clients cannot obtain tokens."
                    checked={draft.enabled ?? false}
                    onChange={(v) => update("enabled", v)}
                  />
                </SwitchList>
              </Section>

              <Section
                title="Authentication & security"
                description="How this client proves its identity."
              >
                <SwitchList>
                  <SwitchRow label="Require client secret" checked={draft.requireClientSecret ?? false} onChange={(v) => update("requireClientSecret", v)} />
                  <SwitchRow label="Require PKCE" checked={draft.requirePkce ?? false} onChange={(v) => update("requirePkce", v)} />
                  <SwitchRow label="Allow plain-text PKCE" warn checked={draft.allowPlainTextPkce ?? false} onChange={(v) => update("allowPlainTextPkce", v)} />
                  <SwitchRow label="Enable local login" checked={draft.enableLocalLogin ?? false} onChange={(v) => update("enableLocalLogin", v)} />
                </SwitchList>
              </Section>

              <Section
                title="Token configuration"
                description="Lifetimes and claim behaviour for issued tokens."
              >
                <FieldGrid>
                  <NumberField label="Access token lifetime (s)" value={Number(draft.accessTokenLifetime) || 0} onChange={(v) => update("accessTokenLifetime", v ?? 0)} />
                  <NumberField label="Identity token lifetime (s)" value={Number(draft.identityTokenLifetime) || 0} onChange={(v) => update("identityTokenLifetime", v ?? 0)} />
                  <NumberField label="Authorization code lifetime (s)" value={Number(draft.authorizationCodeLifetime) || 0} onChange={(v) => update("authorizationCodeLifetime", v ?? 0)} />
                  <NumberField label="Device code lifetime (s)" value={Number(draft.deviceCodeLifetime) || 0} onChange={(v) => update("deviceCodeLifetime", v ?? 0)} />
                </FieldGrid>
                <SwitchList>
                  <SwitchRow label="Allow access tokens via browser" warn checked={draft.allowAccessTokensViaBrowser ?? false} onChange={(v) => update("allowAccessTokensViaBrowser", v)} />
                  <SwitchRow label="Include JWT id" checked={draft.includeJwtId ?? false} onChange={(v) => update("includeJwtId", v)} />
                  <SwitchRow label="Always send client claims" checked={draft.alwaysSendClientClaims ?? false} onChange={(v) => update("alwaysSendClientClaims", v)} />
                  <SwitchRow label="Always include user claims in id token" checked={draft.alwaysIncludeUserClaimsInIdToken ?? false} onChange={(v) => update("alwaysIncludeUserClaimsInIdToken", v)} />
                </SwitchList>
              </Section>

              <Section
                title="Consent & refresh tokens"
                description="User consent prompts and refresh token rotation."
              >
                <FieldGrid>
                  <NumberField label="Consent lifetime (s, blank = never)" nullable value={draft.consentLifetime == null ? null : Number(draft.consentLifetime)} onChange={(v) => update("consentLifetime", v)} />
                  <NumberField label="Absolute refresh token lifetime (s)" value={Number(draft.absoluteRefreshTokenLifetime) || 0} onChange={(v) => update("absoluteRefreshTokenLifetime", v ?? 0)} />
                  <NumberField label="Sliding refresh token lifetime (s)" value={Number(draft.slidingRefreshTokenLifetime) || 0} onChange={(v) => update("slidingRefreshTokenLifetime", v ?? 0)} />
                </FieldGrid>
                <SwitchList>
                  <SwitchRow label="Require consent" checked={draft.requireConsent ?? false} onChange={(v) => update("requireConsent", v)} />
                  <SwitchRow label="Allow remember consent" checked={draft.allowRememberConsent ?? false} onChange={(v) => update("allowRememberConsent", v)} />
                  <SwitchRow label="Allow offline access" checked={draft.allowOfflineAccess ?? false} onChange={(v) => update("allowOfflineAccess", v)} />
                </SwitchList>
              </Section>

              <Section
                title="Advanced"
                description="Logout endpoints, claim prefixes, and other less common options."
              >
                <FieldGrid>
                  <Field label="Back-channel logout URI">
                    <InputText value={draft.backChannelLogoutUri ?? ""} onChange={(e) => update("backChannelLogoutUri", e.target.value || null)} />
                  </Field>
                  <Field label="Front-channel logout URI">
                    <InputText value={draft.frontChannelLogoutUri ?? ""} onChange={(e) => update("frontChannelLogoutUri", e.target.value || null)} />
                  </Field>
                  <Field label="Client claims prefix">
                    <InputText value={draft.clientClaimsPrefix ?? ""} onChange={(e) => update("clientClaimsPrefix", e.target.value || null)} />
                  </Field>
                  <Field label="Pairwise subject salt">
                    <InputText value={draft.pairWiseSubjectSalt ?? ""} onChange={(e) => update("pairWiseSubjectSalt", e.target.value || null)} />
                  </Field>
                  <Field label="User code type">
                    <InputText value={draft.userCodeType ?? ""} onChange={(e) => update("userCodeType", e.target.value || null)} />
                  </Field>
                  <NumberField label="User SSO lifetime (s, blank = default)" nullable value={draft.userSsoLifetime == null ? null : Number(draft.userSsoLifetime)} onChange={(v) => update("userSsoLifetime", v)} />
                </FieldGrid>
                <SwitchList>
                  <SwitchRow label="Back-channel logout session required" checked={draft.backChannelLogoutSessionRequired ?? false} onChange={(v) => update("backChannelLogoutSessionRequired", v)} />
                  <SwitchRow label="Front-channel logout session required" checked={draft.frontChannelLogoutSessionRequired ?? false} onChange={(v) => update("frontChannelLogoutSessionRequired", v)} />
                  <SwitchRow label="Update access token claims on refresh" checked={draft.updateAccessTokenClaimsOnRefresh ?? false} onChange={(v) => update("updateAccessTokenClaimsOnRefresh", v)} />
                  <SwitchRow label="Non-editable" checked={draft.nonEditable ?? false} onChange={(v) => update("nonEditable", v)} />
                </SwitchList>
              </Section>
          </div>
        </TabPanel>

        {!isNew && <TabPanel header="Scopes"><ScopesPanel clientId={numericId} /></TabPanel>}
        {!isNew && <TabPanel header="Grant types"><GrantTypesPanel clientId={numericId} /></TabPanel>}
        {!isNew && <TabPanel header="Redirect URIs"><RedirectUrisPanel clientId={numericId} /></TabPanel>}
        {!isNew && <TabPanel header="Post-logout URIs"><PostLogoutRedirectUrisPanel clientId={numericId} /></TabPanel>}
        {!isNew && <TabPanel header="CORS origins"><CorsOriginsPanel clientId={numericId} /></TabPanel>}
        {!isNew && <TabPanel header="Claims"><ClaimsPanel clientId={numericId} /></TabPanel>}
        {!isNew && <TabPanel header="Secrets"><SecretsPanel clientId={numericId} /></TabPanel>}
        {!isNew && <TabPanel header="Properties"><PropertiesPanel clientId={numericId} /></TabPanel>}
        {!isNew && <TabPanel header="IdP restrictions"><IdpRestrictionsPanel clientId={numericId} /></TabPanel>}
      </TabView>

      <div className="fixed inset-x-0 bottom-0 left-60 z-10 border-t border-border bg-surface/95 backdrop-blur">
        <div className="mx-auto flex w-full max-w-6xl items-center justify-end gap-2 px-6 py-3 md:px-8">
          <Link to="/clients">
            <Button type="button" outlined label="Cancel" />
          </Link>
          <Button
            type="button"
            onClick={submit}
            loading={saving}
            label={isNew ? "Create client" : "Save changes"}
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
  warn,
}: Readonly<{
  label: string;
  description?: string;
  checked: boolean;
  onChange: (next: boolean) => void;
  warn?: boolean;
}>) {
  return (
    <label className="flex cursor-pointer items-center justify-between gap-4 px-4 py-3 transition-colors hover:bg-surface-muted">
      <span className="flex min-w-0 flex-1 items-start gap-2">
        {warn && (
          <FontAwesomeIcon
            icon={faTriangleExclamation}
            className="mt-0.5 text-warning"
            aria-hidden
          />
        )}
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

function NumberField({
  label,
  value,
  onChange,
  nullable,
}: Readonly<{
  label: string;
  value: number | null;
  onChange: (next: number | null) => void;
  nullable?: boolean;
}>) {
  return (
    <Field label={label}>
      <InputNumber
        value={value}
        onChange={(e) => onChange(e.value ?? (nullable ? null : 0))}
        min={0}
        useGrouping={false}
      />
    </Field>
  );
}
