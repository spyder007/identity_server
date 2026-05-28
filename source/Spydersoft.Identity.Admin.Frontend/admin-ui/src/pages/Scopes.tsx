import { Route, Routes } from "react-router-dom";
import ScopesList from "./scopes/ScopesList";
import ScopeEdit from "./scopes/ScopeEdit";

export default function Scopes() {
  return (
    <Routes>
      <Route index element={<ScopesList />} />
      <Route path="new" element={<ScopeEdit />} />
      <Route path=":id/*" element={<ScopeEdit />} />
    </Routes>
  );
}
