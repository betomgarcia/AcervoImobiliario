import { Card, CardContent, type CardProps } from '@mui/material';
import type { ReactNode } from 'react';
import { tokens } from '@/theme/tokens';

interface AppCardProps extends CardProps {
  children: ReactNode;
  noHover?: boolean;
}

/** Card padronizado do Design System. */
export function AppCard({ children, noHover, sx, ...props }: AppCardProps) {
  return (
    <Card
      {...props}
      sx={{
        ...(noHover ? { '&:hover': { boxShadow: tokens.shadow.card } } : {}),
        ...sx,
      }}
    >
      <CardContent sx={{ p: tokens.spacing.cardPadding }}>{children}</CardContent>
    </Card>
  );
}
