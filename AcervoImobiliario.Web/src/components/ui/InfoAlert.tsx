import InfoOutlinedIcon from '@mui/icons-material/InfoOutlined';
import { Alert, type AlertProps } from '@mui/material';
import { tokens } from '@/theme/tokens';

interface InfoAlertProps extends Omit<AlertProps, 'severity'> {
  children: React.ReactNode;
}

/** Alerta informativo padronizado — fundo azul claro, texto legível. */
export function InfoAlert({ children, sx, ...props }: InfoAlertProps) {
  return (
    <Alert
      severity="info"
      icon={<InfoOutlinedIcon />}
      sx={{
        borderRadius: `${tokens.radius.md}px`,
        bgcolor: tokens.color.infoBg,
        color: tokens.color.infoText,
        border: `1px solid ${tokens.color.border}`,
        '& .MuiAlert-icon': { color: tokens.color.primary },
        ...sx,
      }}
      {...props}
    >
      {children}
    </Alert>
  );
}
