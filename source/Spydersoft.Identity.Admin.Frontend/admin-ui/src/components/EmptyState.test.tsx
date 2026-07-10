import { describe, it, expect } from "vitest";
import { render, screen } from "@testing-library/react";
import { faUser } from "@fortawesome/free-solid-svg-icons";
import EmptyState from "./EmptyState";

describe("EmptyState", () => {
  it("renders the title", () => {
    render(<EmptyState title="Nothing here" />);
    expect(screen.getByRole("heading", { name: "Nothing here" })).toBeInTheDocument();
  });

  it("renders the description when provided", () => {
    render(<EmptyState title="Empty" description="Try adding something" />);
    expect(screen.getByText("Try adding something")).toBeInTheDocument();
  });

  it("renders the action node when provided", () => {
    render(<EmptyState title="Empty" action={<button>Add</button>} />);
    expect(screen.getByRole("button", { name: "Add" })).toBeInTheDocument();
  });

  it("renders an icon when provided", () => {
    const { container } = render(<EmptyState title="Empty" icon={faUser} />);
    expect(container.querySelector("svg")).toBeInTheDocument();
  });

  it("uses the roomy (non-compact) container styling by default", () => {
    const { container } = render(<EmptyState title="Empty" />);
    expect(container.firstChild).toHaveClass("border", "bg-surface");
  });

  it("uses compact styling when compact is set", () => {
    const { container } = render(<EmptyState title="Empty" compact />);
    expect(container.firstChild).toHaveClass("py-10");
    expect(container.firstChild).not.toHaveClass("bg-surface");
  });
});
