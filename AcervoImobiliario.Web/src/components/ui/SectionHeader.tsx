import { Box, Stack, Typography } from '@mui/material';
import type { ReactNode } from 'react';
import { tokens } from '@/theme/tokens';

interface SectionHeaderProps {
  title: string;
  description?: string;
  action?: ReactNode;
  accent?: boolean;
}

export function SectionHeader({ title, description, action, accent = true }: SectionHeaderProps) {
  return (
    <Stack
      direction={{ xs: 'column', sm: 'row' }}
      alignItems={{ xs: 'flex-start', sm: 'center' }}
      justifyContent="space-between"
      spacing={1.5}
      sx={{ mb: 2.5 }}
    >
      <Box sx={{ display: 'flex', gap: 1.5, alignItems: 'flex-start', flex: 1 }}>
        {accent ? (
          <Box
            sx={{
              width: 4,
              minHeight: 40,
              borderRadius: tokens.radius.pill,
              bgcolor: 'primary.main',
              flexShrink: 0,
              mt: 0.25,
            }}
          />
        ) : null}
        <Box>
          <Typography variant="h6" component="h2">
            {title}
          </Typography>
          {description ? (
            <Typography variant="body2" sx={{ mt: 0.5, maxWidth: 640 }}>
              {description}
            </Typography>
          ) : null}
        </Box>
      </Box>
      {action}
    </Stack>
  );
}
