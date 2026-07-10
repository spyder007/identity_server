import { describe, it, expect } from "vitest";
import { render, screen } from "@testing-library/react";
import { faGear } from "@fortawesome/free-solid-svg-icons";
import PageHeader from "./PageHeader";

describe("PageHeader", () => {
  it("renders the title as a level-1 heading", () => {
    render(<PageHeader title="Clients" />);
    expect(screen.getByRole("heading", { level: 1, name: "Clients" })).toBeInTheDocument();
  });

  it("renders the subtitle, eyebrow, and actions when provided", () => {
    render(
      <PageHeader
        title="Clients"
        subtitle="Manage your clients"
        eyebrow="Configuration"
        actions={<button>New</button>}
      />,
    );
    expect(screen.getByText("Manage your clients")).toBeInTheDocument();
    expect(screen.getByText("Configuration")).toBeInTheDocument();
    expect(screen.getByRole("button", { name: "New" })).toBeInTheDocument();
  });

  it("renders an icon when provided", () => {
    const { container } = render(<PageHeader title="Settings" icon={faGear} />);
    expect(container.querySelector("svg")).toBeInTheDocument();
  });

  it("omits optional regions when not provided", () => {
    render(<PageHeader title="Clients" />);
    expect(screen.queryByRole("button")).not.toBeInTheDocument();
  });
});
