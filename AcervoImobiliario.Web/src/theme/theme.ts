import { alpha, createTheme } from '@mui/material/styles';
import { tokens } from '@/theme/tokens';

declare module '@mui/material/styles' {
  interface Palette {
    highlight: Palette['primary'];
  }
  interface PaletteOptions {
    highlight?: PaletteOptions['primary'];
  }
}

declare module '@mui/material/Chip' {
  interface ChipPropsColorOverrides {
    highlight: true;
  }
}

export const theme = createTheme({
  palette: {
    mode: 'light',
    primary: {
      main: tokens.color.primary,
      light: tokens.color.primaryLight,
      dark: tokens.color.primaryDark,
      contrastText: tokens.color.textOnPrimary,
    },
    secondary: {
      main: tokens.color.accent,
      dark: tokens.color.accentDark,
      contrastText: tokens.color.accentContrast,
    },
    highlight: {
      main: tokens.color.accent,
      dark: tokens.color.accentDark,
      contrastText: tokens.color.accentContrast,
    },
    success: {
      main: tokens.color.success,
      light: tokens.color.successLight,
      contrastText: '#FFFFFF',
    },
    warning: {
      main: tokens.color.warning,
      light: tokens.color.warningLight,
      contrastText: tokens.color.accentContrast,
    },
    error: {
      main: tokens.color.error,
      light: tokens.color.errorLight,
      contrastText: '#FFFFFF',
    },
    background: {
      default: tokens.color.background,
      paper: tokens.color.surface,
    },
    text: {
      primary: tokens.color.textPrimary,
      secondary: tokens.color.textSecondary,
      disabled: tokens.color.textDisabled,
    },
    divider: tokens.color.border,
  },
  shape: { borderRadius: tokens.radius.md },
  typography: {
    fontFamily: tokens.typography.fontFamily,
    h4: {
      fontWeight: 700,
      letterSpacing: '-0.02em',
      color: tokens.color.textPrimary,
      fontSize: '1.75rem',
    },
    h5: { fontWeight: 700, color: tokens.color.textPrimary },
    h6: { fontWeight: 600, color: tokens.color.textPrimary },
    subtitle1: { fontWeight: 600, color: tokens.color.textPrimary },
    subtitle2: { fontWeight: 600, color: tokens.color.textPrimary },
    body1: { color: tokens.color.textPrimary, lineHeight: 1.6 },
    body2: { color: tokens.color.textSecondary, lineHeight: 1.55 },
    caption: { color: tokens.color.textSecondary },
    overline: {
      letterSpacing: '0.06em',
      fontWeight: 600,
      color: tokens.color.textSecondary,
      fontSize: '0.7rem',
    },
    button: { textTransform: 'none', fontWeight: 600 },
  },
  components: {
    MuiCssBaseline: {
      styleOverrides: {
        body: { backgroundColor: tokens.color.background },
      },
    },
    MuiButton: {
      defaultProps: { disableElevation: true },
      styleOverrides: {
        root: {
          borderRadius: tokens.radius.md,
          paddingInline: 20,
          minHeight: 44,
          transition: tokens.transition.default,
        },
        containedPrimary: {
          '&:hover': { backgroundColor: tokens.color.primaryDark },
        },
        outlinedPrimary: {
          borderWidth: 1.5,
          borderColor: tokens.color.primary,
          color: tokens.color.primary,
          backgroundColor: tokens.color.surface,
          '&:hover': {
            borderWidth: 1.5,
            backgroundColor: tokens.color.navHover,
          },
        },
        textPrimary: {
          '&:hover': { backgroundColor: tokens.color.navHover },
        },
        sizeLarge: {
          minHeight: 48,
          paddingInline: 24,
        },
      },
    },
    MuiCard: {
      defaultProps: { elevation: 0 },
      styleOverrides: {
        root: {
          borderRadius: tokens.radius.lg,
          border: `1px solid ${tokens.color.border}`,
          boxShadow: tokens.shadow.card,
          backgroundColor: tokens.color.surface,
        },
      },
    },
    MuiAppBar: {
      styleOverrides: {
        root: {
          backgroundColor: tokens.color.primary,
          backgroundImage: 'none',
          boxShadow: tokens.shadow.appBar,
          color: tokens.color.textOnPrimary,
        },
      },
    },
    MuiTextField: {
      defaultProps: { variant: 'outlined', size: 'medium' },
      styleOverrides: {
        root: {
          '& .MuiInputLabel-root': {
            color: tokens.color.textSecondary,
            '&.Mui-focused': { color: tokens.color.primary },
          },
          '& .MuiOutlinedInput-root': {
            borderRadius: tokens.radius.md,
            minHeight: tokens.spacing.fieldHeight,
            backgroundColor: tokens.color.surface,
            '& fieldset': { borderColor: tokens.color.border },
            '&:hover fieldset': { borderColor: alpha(tokens.color.primary, 0.5) },
            '&.Mui-focused fieldset': {
              borderWidth: 2,
              borderColor: tokens.color.borderFocus,
            },
          },
          '& .MuiFormHelperText-root': {
            color: tokens.color.textSecondary,
            marginTop: 6,
          },
        },
      },
    },
    MuiAutocomplete: {
      styleOverrides: {
        root: {
          '& .MuiOutlinedInput-root': { minHeight: tokens.spacing.fieldHeight },
        },
      },
    },
    MuiFormControl: {
      styleOverrides: {
        root: {
          '& .MuiInputLabel-root': { color: tokens.color.textSecondary },
          '& .MuiOutlinedInput-root': {
            borderRadius: tokens.radius.md,
            minHeight: tokens.spacing.fieldHeight,
            backgroundColor: tokens.color.surface,
          },
        },
      },
    },
    MuiChip: {
      styleOverrides: {
        root: {
          fontWeight: 600,
          borderRadius: tokens.radius.sm,
          '&.MuiChip-colorHighlight': {
            backgroundColor: tokens.color.accent,
            color: tokens.color.accentContrast,
          },
        },
        outlined: {
          borderColor: tokens.color.border,
        },
      },
    },
    MuiAlert: {
      styleOverrides: {
        root: { borderRadius: tokens.radius.md },
        standardSuccess: {
          backgroundColor: tokens.color.successLight,
          color: tokens.color.success,
        },
        standardInfo: {
          backgroundColor: tokens.color.infoBg,
          color: tokens.color.infoText,
          '& .MuiAlert-icon': { color: tokens.color.primary },
        },
        standardError: {
          backgroundColor: tokens.color.errorLight,
          color: tokens.color.error,
        },
      },
    },
    MuiTableHead: {
      styleOverrides: {
        root: {
          '& .MuiTableCell-head': {
            fontWeight: 700,
            fontSize: '0.8125rem',
            textTransform: 'uppercase',
            letterSpacing: '0.04em',
            color: tokens.color.textSecondary,
            backgroundColor: tokens.color.background,
            borderBottom: `2px solid ${tokens.color.border}`,
          },
        },
      },
    },
    MuiTableRow: {
      styleOverrides: {
        root: {
          '&:nth-of-type(even)': { backgroundColor: tokens.color.tableRowStripe },
          '&:hover': { backgroundColor: tokens.color.tableRowHover },
        },
      },
    },
    MuiTableCell: {
      styleOverrides: {
        root: {
          borderColor: tokens.color.border,
          py: 1.75,
          color: tokens.color.textPrimary,
        },
      },
    },
    MuiStepIcon: {
      styleOverrides: {
        root: {
          '&.Mui-completed': { color: tokens.color.primary },
          '&.Mui-active': { color: tokens.color.primary },
        },
      },
    },
    MuiDivider: {
      styleOverrides: {
        root: { borderColor: tokens.color.border },
      },
    },
    MuiDrawer: {
      styleOverrides: {
        paper: {
          borderRight: `1px solid ${tokens.color.border}`,
          backgroundColor: tokens.color.surface,
        },
      },
    },
    MuiToggleButton: {
      styleOverrides: {
        root: {
          textTransform: 'none',
          fontWeight: 600,
          color: tokens.color.textSecondary,
          borderColor: tokens.color.border,
          '&.Mui-selected': {
            backgroundColor: tokens.color.navHover,
            color: tokens.color.primary,
            borderColor: tokens.color.primary,
            '&:hover': { backgroundColor: alpha(tokens.color.primary, 0.12) },
          },
        },
      },
    },
    MuiFab: {
      styleOverrides: {
        primary: { boxShadow: tokens.shadow.fab },
      },
    },
  },
});
