import AddIcon from '@mui/icons-material/Add';
import HistoryEduIcon from '@mui/icons-material/HistoryEdu';
import LocationOnIcon from '@mui/icons-material/LocationOn';
import {
  Box,
  Button,
  Chip,
  Divider,
  Grid,
  Stack,
  Typography,
} from '@mui/material';
import { Link as RouterLink, useParams } from 'react-router-dom';
import { QueryState } from '@/components/common/QueryState';
import { PageHeader } from '@/components/common/PageHeader';
import { AppCard } from '@/components/ui/AppCard';
import { InfoField } from '@/components/ui/InfoField';
import { StatusChip } from '@/components/ui/StatusChip';
import { useProperty } from '@/hooks/useProperties';
import { formatAddress, formatDateTime, formatFullPropertyAddress } from '@/utils/format';
import { tokens } from '@/theme/tokens';

export function PropertyDetailPage() {
  const { id = '' } = useParams();
  const propertyQuery = useProperty(id);

  return (
    <QueryState
      isLoading={propertyQuery.isLoading}
      error={propertyQuery.error}
      data={propertyQuery.data}
      loadingMessage="Carregando imóvel..."
      emptyTitle="Imóvel não encontrado"
      emptyDescription="O imóvel solicitado não existe ou foi removido."
      isEmpty={(property) => !property}
    >
      {(property) => {
        const complementText = property.complement?.trim() || 'Sem complemento';
        const fullAddress = formatFullPropertyAddress(property);

        return (
          <Stack spacing={3}>
            <PageHeader
              title="Detalhes do imóvel"
              subtitle={formatAddress(
                property.street,
                property.number,
                property.neighborhood,
                property.cityNameSnapshot,
              )}
            />

            <AppCard
              noHover
              sx={{
                borderLeft: `4px solid ${tokens.color.primary}`,
              }}
            >
                <Stack direction="row" spacing={1.5} alignItems="flex-start" sx={{ mb: 2 }}>
                  <LocationOnIcon color="primary" sx={{ mt: 0.25 }} />
                  <Box>
                    <Typography variant="h5" sx={{ lineHeight: 1.35 }}>
                      {fullAddress}
                    </Typography>
                    <Stack direction="row" spacing={1} flexWrap="wrap" useFlexGap sx={{ mt: 1.5 }}>
                      <StatusChip active={property.isActive} />
                    </Stack>
                  </Box>
                </Stack>

                <Grid container spacing={2}>
                  <Grid item xs={12} sm={6} md={4}>
                    <InfoField label="Cidade" value={property.cityNameSnapshot} />
                  </Grid>
                  <Grid item xs={12} sm={6} md={4}>
                    <InfoField label="Bairro" value={property.neighborhood} />
                  </Grid>
                  <Grid item xs={12} sm={6} md={4}>
                    <InfoField label="Rua" value={property.street} />
                  </Grid>
                  <Grid item xs={12} sm={6} md={4}>
                    <InfoField label="Número" value={property.number} />
                  </Grid>
                  <Grid item xs={12} sm={6} md={4}>
                    <InfoField label="Complemento" value={complementText} />
                  </Grid>
                  <Grid item xs={12} sm={6} md={4}>
                    <InfoField
                      label="Índice cadastral"
                      value={property.cadastralIndex ?? 'Não informado'}
                    />
                  </Grid>
                </Grid>

                <Divider sx={{ my: 2.5 }} />

                <Stack direction="row" spacing={1} flexWrap="wrap" useFlexGap>
                  <Chip
                    variant="outlined"
                    size="small"
                    label={`Cadastrado em ${formatDateTime(property.createdAt)}`}
                  />
                  {property.updatedAt ? (
                    <Chip
                      variant="outlined"
                      size="small"
                      label={`Atualizado em ${formatDateTime(property.updatedAt)}`}
                    />
                  ) : null}
                </Stack>
            </AppCard>

            <AppCard noHover>
                <Typography variant="subtitle1" sx={{ mb: 2 }}>
                  Ações
                </Typography>
                <Stack direction={{ xs: 'column', sm: 'row' }} spacing={1.5}>
                  <Button
                    component={RouterLink}
                    to={`/imoveis/${property.id}/historico`}
                    variant="outlined"
                    color="primary"
                    startIcon={<HistoryEduIcon />}
                    fullWidth
                  >
                    Ver histórico
                  </Button>
                  <Button
                    component={RouterLink}
                    to={`/imoveis/${property.id}/historico/novo`}
                    variant="contained"
                    color="primary"
                    startIcon={<AddIcon />}
                    fullWidth
                  >
                    Adicionar histórico
                  </Button>
                </Stack>
            </AppCard>
          </Stack>
        );
      }}
    </QueryState>
  );
}
