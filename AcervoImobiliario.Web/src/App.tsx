import { CssBaseline, ThemeProvider } from '@mui/material';
import { QueryClient, QueryClientProvider } from '@tanstack/react-query';
import { PwaInstallProvider } from '@/components/pwa/PwaInstallContext';
import { PwaInstallPrompt } from '@/components/pwa/PwaInstallPrompt';
import { AppRoutes } from '@/routes/AppRoutes';
import { theme } from '@/theme';

const queryClient = new QueryClient({
  defaultOptions: {
    queries: {
      retry: 1,
      refetchOnWindowFocus: false,
      staleTime: 30_000,
    },
  },
});

export default function App() {
  return (
    <QueryClientProvider client={queryClient}>
      <ThemeProvider theme={theme}>
        <CssBaseline />
        <PwaInstallProvider>
          <AppRoutes />
          <PwaInstallPrompt />
        </PwaInstallProvider>
      </ThemeProvider>
    </QueryClientProvider>
  );
}
