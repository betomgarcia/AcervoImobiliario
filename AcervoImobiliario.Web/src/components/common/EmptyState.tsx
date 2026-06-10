import SearchOffIcon from '@mui/icons-material/SearchOff';
import { Box, Typography } from '@mui/material';
import { tokens } from '@/theme/tokens';

interface EmptyStateProps {
  title: string;
  description?: string;
}

export function EmptyState({ title, description }: EmptyStateProps) {
  return (
    <Box
      sx={{
        py: 8,
        px: 3,
        textAlign: 'center',
        borderRadius: `${tokens.radius.lg}px`,
        border: `1px dashed ${tokens.color.borderStrong}`,
        bgcolor: 'background.paper',
      }}
    >
      <SearchOffIcon sx={{ fontSize: 48, color: 'primary.main', opacity: 0.5, mb: 1 }} />
      <Typography variant="h6" gutterBottom>
        {title}
      </Typography>
      {description ? (
        <Typography variant="body2" color="text.secondary" sx={{ maxWidth: 480, mx: 'auto' }}>
          {description}
        </Typography>
      ) : null}
    </Box>
  );
}
