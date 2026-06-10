import GetAppIcon from '@mui/icons-material/GetApp';
import IosShareIcon from '@mui/icons-material/IosShare';
import PhoneAndroidIcon from '@mui/icons-material/PhoneAndroid';
import {
  Box,
  Button,
  Fab,
  Paper,
  Stack,
  Typography,
  useMediaQuery,
  useTheme,
} from '@mui/material';
import { useCallback, useEffect, useState } from 'react';
import { usePwaInstallLayout } from '@/components/pwa/PwaInstallContext';
import { readPwaInstallMinimized, writePwaInstallMinimized } from '@/components/pwa/pwaInstallStorage';
import { tokens } from '@/theme/tokens';

type BeforeInstallPromptEvent = Event & {
  prompt: () => Promise<void>;
  userChoice: Promise<{ outcome: 'accepted' | 'dismissed' }>;
};

const EXPANDED_BOTTOM_OFFSET_MOBILE = 168;
const EXPANDED_BOTTOM_OFFSET_DESKTOP = 152;
const MINIMIZED_BOTTOM_OFFSET = 88;

function isIosDevice(): boolean {
  return /iphone|ipad|ipod/i.test(navigator.userAgent);
}

function isStandaloneMode(): boolean {
  return (
    window.matchMedia('(display-mode: standalone)').matches ||
    ('standalone' in navigator &&
      (navigator as Navigator & { standalone?: boolean }).standalone === true)
  );
}

export function PwaInstallPrompt() {
  const theme = useTheme();
  const isMobile = useMediaQuery(theme.breakpoints.down('sm'));
  const { setBottomOffset } = usePwaInstallLayout();

  const [deferredPrompt, setDeferredPrompt] = useState<BeforeInstallPromptEvent | null>(null);
  const [eligible, setEligible] = useState(false);
  const [expanded, setExpanded] = useState(() => !readPwaInstallMinimized());
  const [showIosHint, setShowIosHint] = useState(false);

  const updateBottomOffset = useCallback(
    (isExpanded: boolean, isEligible: boolean) => {
      if (!isEligible) {
        setBottomOffset(0);
        return;
      }

      if (isExpanded) {
        setBottomOffset(isMobile ? EXPANDED_BOTTOM_OFFSET_MOBILE : EXPANDED_BOTTOM_OFFSET_DESKTOP);
        return;
      }

      setBottomOffset(MINIMIZED_BOTTOM_OFFSET);
    },
    [isMobile, setBottomOffset],
  );

  useEffect(() => {
    if (isStandaloneMode()) {
      setEligible(false);
      setBottomOffset(0);
      return;
    }

    if (isIosDevice()) {
      setShowIosHint(true);
      setEligible(true);
      return;
    }

    const handleBeforeInstall = (event: Event) => {
      event.preventDefault();
      setDeferredPrompt(event as BeforeInstallPromptEvent);
      setEligible(true);
    };

    const handleAppInstalled = () => {
      setDeferredPrompt(null);
      setEligible(false);
      setBottomOffset(0);
    };

    window.addEventListener('beforeinstallprompt', handleBeforeInstall);
    window.addEventListener('appinstalled', handleAppInstalled);

    return () => {
      window.removeEventListener('beforeinstallprompt', handleBeforeInstall);
      window.removeEventListener('appinstalled', handleAppInstalled);
    };
  }, [setBottomOffset]);

  useEffect(() => {
    updateBottomOffset(expanded, eligible);
  }, [expanded, eligible, updateBottomOffset]);

  const handleMinimize = () => {
    setExpanded(false);
    writePwaInstallMinimized(true);
  };

  const handleExpand = () => {
    setExpanded(true);
    writePwaInstallMinimized(false);
  };

  const handleInstall = async () => {
    if (!deferredPrompt) {
      return;
    }

    await deferredPrompt.prompt();
    const choice = await deferredPrompt.userChoice;
    setDeferredPrompt(null);

    if (choice.outcome === 'accepted') {
      setEligible(false);
      setBottomOffset(0);
    }
  };

  if (!eligible) {
    return null;
  }

  const anchorSx = {
    position: 'fixed' as const,
    right: { xs: 12, sm: 20 },
    bottom: `calc(${theme.spacing(2)} + env(safe-area-inset-bottom, 0px))`,
    zIndex: theme.zIndex.snackbar,
    maxWidth: { xs: 'min(360px, calc(100vw - 24px))', sm: 360 },
  };

  if (!expanded) {
    return (
      <Fab
        variant="extended"
        color="primary"
        onClick={handleExpand}
        aria-label="Instalar app"
        sx={{
          ...anchorSx,
          height: 44,
          px: 2,
          borderRadius: tokens.radius.pill,
          boxShadow: tokens.shadow.fab,
          textTransform: 'none',
          fontWeight: 600,
        }}
      >
        <PhoneAndroidIcon sx={{ mr: 1 }} />
        Instalar app
      </Fab>
    );
  }

  return (
    <Paper
      elevation={8}
      sx={{
        ...anchorSx,
        p: 2,
        borderRadius: `${tokens.radius.lg}px`,
        border: `1px solid ${tokens.color.borderStrong}`,
        bgcolor: 'background.paper',
      }}
    >
      <Stack spacing={1.5}>
        <Box>
          <Typography variant="subtitle1" fontWeight={700} color="primary.main">
            Instalar Acervo Imobiliário
          </Typography>
          <Typography variant="body2" color="text.secondary" sx={{ mt: 0.5 }}>
            Acesse rapidamente pelo computador, tablet ou celular.
          </Typography>
        </Box>

        {showIosHint ? (
          <Typography variant="body2" color="text.secondary">
            No iPhone, toque em <strong>Compartilhar</strong>{' '}
            <IosShareIcon sx={{ fontSize: 16, verticalAlign: 'text-bottom' }} /> e depois em{' '}
            <strong>Adicionar à Tela de Início</strong>.
          </Typography>
        ) : null}

        <Stack direction="row" spacing={1} justifyContent="flex-end">
          <Button variant="text" color="inherit" size="small" onClick={handleMinimize}>
            Minimizar
          </Button>
          {!showIosHint && deferredPrompt ? (
            <Button
              variant="contained"
              color="primary"
              size="small"
              startIcon={<GetAppIcon />}
              onClick={handleInstall}
            >
              Instalar
            </Button>
          ) : showIosHint ? (
            <Button variant="contained" size="small" onClick={handleMinimize}>
              Entendi
            </Button>
          ) : null}
        </Stack>
      </Stack>
    </Paper>
  );
}
