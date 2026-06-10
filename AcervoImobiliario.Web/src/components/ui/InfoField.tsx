import { Box, Typography } from '@mui/material';

interface InfoFieldProps {
  label: string;
  value: string;
}

/** Campo de informação para telas de detalhe — label + valor legível. */
export function InfoField({ label, value }: InfoFieldProps) {
  return (
    <Box
      sx={{
        p: 2,
        borderRadius: 2,
        border: '1px solid',
        borderColor: 'divider',
        bgcolor: 'background.default',
        height: '100%',
      }}
    >
      <Typography variant="overline" display="block" sx={{ mb: 0.5 }}>
        {label}
      </Typography>
      <Typography variant="body1" fontWeight={600}>
        {value}
      </Typography>
    </Box>
  );
}
