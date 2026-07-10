import { Route, Routes } from "react-router-dom";
import ClientsList from "./clients/ClientsList";
import ClientEdit from "./clients/ClientEdit";

export default function Clients() {
  return (
    <Routes>
      <Route index element={<ClientsList />} />
      <Route path="new" element={<ClientEdit />} />
      <Route path=":id/*" element={<ClientEdit />} />
    </Routes>
  );
}
