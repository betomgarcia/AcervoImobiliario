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
import { ComplementType, type PropertyResponse } from '@/types/api';
import { formatDateTime, formatFullPropertyAddress } from '@/utils/format';
import { complementTypeLabels } from '@/utils/labels';

interface PropertySummaryCardProps {
  property: PropertyResponse;
}

export function PropertySummaryCard({ property }: PropertySummaryCardProps) {
  const fullAddress = formatFullPropertyAddress(property);

  const complementText =
    property.complementType === ComplementType.None
      ? 'Sem complemento'
      : `${complementTypeLabels[property.complementType]}${property.complementValue ? `: ${property.complementValue}` : ''}`;

  return (
    <Card
      sx={{
        overflow: 'hidden',
        background: 'linear-gradient(135deg, #0D47A1 0%, #1565C0 60%, #00838F 100%)',
        color: 'primary.contrastText',
      }}
    >
      <CardContent sx={{ p: { xs: 2, md: 3 } }}>
        <Stack direction="row" spacing={1.5} alignItems="flex-start" sx={{ mb: 2 }}>
          <Box
            sx={{
              width: 44,
              height: 44,
              borderRadius: 2,
              bgcolor: 'rgba(255,255,255,0.15)',
              display: 'flex',
              alignItems: 'center',
              justifyContent: 'center',
              flexShrink: 0,
            }}
          >
            <HomeWorkIcon />
          </Box>
          <Box sx={{ flex: 1 }}>
            <Typography variant="overline" sx={{ opacity: 0.9, letterSpacing: 1 }}>
              Imóvel
            </Typography>
            <Stack direction="row" spacing={1} alignItems="flex-start" sx={{ mt: 0.5 }}>
              <LocationOnIcon sx={{ fontSize: 20, mt: 0.25, opacity: 0.9 }} />
              <Typography variant="h6" fontWeight={700} sx={{ lineHeight: 1.35 }}>
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
          <Chip
            size="small"
            label={property.isActive ? 'Ativo' : 'Inativo'}
            sx={{
              bgcolor: property.isActive ? 'rgba(76,175,80,0.25)' : 'rgba(255,255,255,0.15)',
              color: 'inherit',
              fontWeight: 600,
            }}
          />
          <Chip
            size="small"
            variant="outlined"
            label={`Cadastrado em ${formatDateTime(property.createdAt)}`}
            sx={{ borderColor: 'rgba(255,255,255,0.4)', color: 'inherit' }}
          />
        </Stack>
      </CardContent>
    </Card>
  );
}

function SummaryItem({ label, value }: { label: string; value: string }) {
  return (
    <Box>
      <Typography variant="caption" sx={{ opacity: 0.8, display: 'block' }}>
        {label}
      </Typography>
      <Typography variant="body2" fontWeight={600}>
        {value}
      </Typography>
    </Box>
  );
}
