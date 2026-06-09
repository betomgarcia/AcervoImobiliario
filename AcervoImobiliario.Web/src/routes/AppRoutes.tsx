import { BrowserRouter, Navigate, Route, Routes } from 'react-router-dom';
import { AppLayout } from '@/components/layout/AppLayout';
import { PropertyCreatePage } from '@/pages/PropertyCreatePage';
import { PropertyDetailPage } from '@/pages/PropertyDetailPage';
import { PropertyHistoryCreatePage } from '@/pages/PropertyHistoryCreatePage';
import { PropertyHistoryPage } from '@/pages/PropertyHistoryPage';
import { PropertySearchPage } from '@/pages/PropertySearchPage';

export function AppRoutes() {
  return (
    <BrowserRouter>
      <Routes>
        <Route element={<AppLayout />}>
          <Route index element={<PropertySearchPage />} />
          <Route path="imoveis/novo" element={<PropertyCreatePage />} />
          <Route path="imoveis/:id" element={<PropertyDetailPage />} />
          <Route path="imoveis/:id/historico" element={<PropertyHistoryPage />} />
          <Route path="imoveis/:id/historico/novo" element={<PropertyHistoryCreatePage />} />
          <Route path="*" element={<Navigate to="/" replace />} />
        </Route>
      </Routes>
    </BrowserRouter>
  );
}
