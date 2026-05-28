import { Route, Routes } from "react-router-dom";
import ApiResourcesList from "./api-resources/ApiResourcesList";
import ApiResourceEdit from "./api-resources/ApiResourceEdit";

export default function ApiResources() {
  return (
    <Routes>
      <Route index element={<ApiResourcesList />} />
      <Route path="new" element={<ApiResourceEdit />} />
      <Route path=":id/*" element={<ApiResourceEdit />} />
    </Routes>
  );
}
