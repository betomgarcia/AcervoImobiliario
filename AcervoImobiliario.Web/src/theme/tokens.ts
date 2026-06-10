/**
 * Design System — Acervo Imobiliário
 * Tokens visuais oficiais. Use via tema MUI ou CSS variables.
 */
export const tokens = {
  color: {
    primary: '#0054B3',
    primaryDark: '#003D82',
    primaryLight: '#0068D9',
    accent: '#F1E64C',
    accentDark: '#D4C842',
    accentContrast: '#0F1F3D',

    success: '#2E7D32',
    successLight: '#E8F5E9',
    warning: '#EDB800',
    warningLight: '#FFF8E1',
    error: '#C62828',
    errorLight: '#FFEBEE',

    background: '#F4F7FB',
    surface: '#FFFFFF',

    textPrimary: '#0F1F3D',
    textSecondary: '#5F6B7A',
    textDisabled: '#8B95A5',
    textOnPrimary: '#FFFFFF',
    textAccentOnPrimary: '#F1E64C',

    border: '#D8E0EA',
    borderStrong: '#B8C5D4',
    borderFocus: '#0054B3',

    navHover: 'rgba(0, 84, 179, 0.06)',
    infoBg: 'rgba(0, 84, 179, 0.08)',
    infoText: '#003D82',
    tableRowHover: 'rgba(0, 84, 179, 0.04)',
    tableRowStripe: 'rgba(0, 84, 179, 0.02)',
  },

  gradient: {
    hero: 'linear-gradient(135deg, #0054B3 0%, #0068D9 100%)',
    timeline: 'linear-gradient(180deg, #0054B3 0%, #0068D9 100%)',
  },

  shadow: {
    card: '0 2px 12px rgba(15, 31, 61, 0.06)',
    cardHover: '0 6px 20px rgba(15, 31, 61, 0.1)',
    fab: '0 8px 24px rgba(0, 84, 179, 0.28)',
    appBar: '0 2px 8px rgba(0, 61, 130, 0.15)',
  },

  radius: {
    sm: 8,
    md: 12,
    lg: 16,
    xl: 20,
    pill: 999,
  },

  layout: {
    maxContentWidth: 1200,
    drawerWidth: 260,
  },

  spacing: {
    fieldHeight: 48,
    cardPadding: { xs: 2, md: 3 },
    sectionGap: 3,
  },

  typography: {
    fontFamily: '"Inter", "Segoe UI", "Roboto", "Helvetica", "Arial", sans-serif',
  },

  transition: {
    default: '0.2s ease',
    nav: 'background-color 0.2s ease, color 0.2s ease',
  },
} as const;

export type DesignTokens = typeof tokens;
