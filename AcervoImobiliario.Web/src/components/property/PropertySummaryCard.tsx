import HomeWorkIcon from '@mui/icons-material/HomeWork';
import LocationOnIcon from '@mui/icons-material/LocationOn';
import {
  Box,
  Card,
  CardContent,
  Chip,
  Grid,
  Stack,
  Typography,
} from '@mui/material';
import { StatusChip } from '@/components/ui/StatusChip';
import type { PropertyResponse } from '@/types/api';
import { formatDateTime, formatFullPropertyAddress } from '@/utils/format';
import { tokens } from '@/theme/tokens';

interface PropertySummaryCardProps {
  property: PropertyResponse;
}

export function PropertySummaryCard({ property }: PropertySummaryCardProps) {
  const fullAddress = formatFullPropertyAddress(property);
  const complementText = property.complement?.trim() || 'Sem complemento';

  return (
    <Card
      sx={{
        overflow: 'hidden',
        background: tokens.gradient.hero,
        color: 'primary.contrastText',
        boxShadow: tokens.shadow.cardHover,
        '&:hover': { boxShadow: tokens.shadow.cardHover },
      }}
    >
      <CardContent sx={{ p: { xs: 2, md: 3 } }}>
        <Stack direction="row" spacing={1.5} alignItems="flex-start" sx={{ mb: 2 }}>
          <Box
            sx={{
              width: 48,
              height: 48,
              borderRadius: `${tokens.radius.md}px`,
              bgcolor: 'rgba(255,255,255,0.18)',
              display: 'flex',
              alignItems: 'center',
              justifyContent: 'center',
              flexShrink: 0,
            }}
          >
            <HomeWorkIcon />
          </Box>
          <Box sx={{ flex: 1 }}>
            <Typography variant="overline" sx={{ opacity: 0.92, letterSpacing: 1 }}>
              Imóvel
            </Typography>
            <Stack direction="row" spacing={1} alignItems="flex-start" sx={{ mt: 0.5 }}>
              <LocationOnIcon sx={{ fontSize: 20, mt: 0.25, opacity: 0.92 }} />
              <Typography variant="h6" sx={{ lineHeight: 1.35 }}>
                {fullAddress}
              </Typography>
            </Stack>
          </Box>
        </Stack>

        <Grid container spacing={2}>
          <Grid item xs={12} sm={6} md={3}>
            <SummaryItem label="Cidade" value={property.cityNameSnapshot} />
          </Grid>
          <Grid item xs={12} sm={6} md={3}>
            <SummaryItem label="Bairro" value={property.neighborhood} />
          </Grid>
          <Grid item xs={12} sm={6} md={3}>
            <SummaryItem label="Complemento" value={complementText} />
          </Grid>
          <Grid item xs={12} sm={6} md={3}>
            <SummaryItem
              label="Índice cadastral"
              value={property.cadastralIndex ?? 'Não informado'}
            />
          </Grid>
        </Grid>

        <Stack direction="row" spacing={1} flexWrap="wrap" useFlexGap sx={{ mt: 2 }}>
          <StatusChip active={property.isActive} />
          <Chip
            size="small"
            variant="outlined"
            label={`Cadastrado em ${formatDateTime(property.createdAt)}`}
            sx={{
              borderColor: 'rgba(255,255,255,0.45)',
              color: 'inherit',
              fontWeight: 600,
            }}
          />
        </Stack>
      </CardContent>
    </Card>
  );
}

function SummaryItem({ label, value }: { label: string; value: string }) {
  return (
    <Box>
      <Typography variant="caption" sx={{ opacity: 0.85, display: 'block' }}>
        {label}
      </Typography>
      <Typography variant="body2" fontWeight={600}>
        {value}
      </Typography>
    </Box>
  );
}
