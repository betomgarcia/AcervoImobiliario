import HistoryEduIcon from '@mui/icons-material/HistoryEdu';
import LocationOnIcon from '@mui/icons-material/LocationOn';
import {
  Box,
  Button,
  Card,
  CardContent,
  Chip,
  Divider,
  Grid,
  Stack,
  Typography,
} from '@mui/material';
import { Link as RouterLink, useParams } from 'react-router-dom';
import { QueryState } from '@/components/common/QueryState';
import { PageHeader } from '@/components/common/PageHeader';
import { useProperty } from '@/hooks/useProperties';
import { formatAddress, formatDateTime } from '@/utils/format';
import { complementTypeLabels } from '@/utils/labels';
import { ComplementType } from '@/types/api';

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
        const complementText =
          property.complementType === ComplementType.None
            ? 'Sem complemento'
            : `${complementTypeLabels[property.complementType]}${property.complementValue ? `: ${property.complementValue}` : ''}`;

        return (
          <Stack spacing={3}>
            <PageHeader
              title="Detalhe do imóvel"
              subtitle={formatAddress(
                property.street,
                property.number,
                property.neighborhood,
                property.cityNameSnapshot,
              )}
              action={
                <Button
                  component={RouterLink}
                  to={`/imoveis/${property.id}/historico`}
                  variant="contained"
                  startIcon={<HistoryEduIcon />}
                >
                  Ver histórico
                </Button>
              }
            />

            <Card>
              <CardContent sx={{ p: { xs: 2, md: 3 } }}>
                <Stack direction="row" spacing={1} alignItems="center" sx={{ mb: 3 }}>
                  <LocationOnIcon color="primary" />
                  <Typography variant="h5">Endereço</Typography>
                </Stack>

                <Grid container spacing={2}>
                  <Grid item xs={12} md={6}>
                    <InfoItem label="Cidade" value={property.cityNameSnapshot} />
                  </Grid>
                  <Grid item xs={12} md={6}>
                    <InfoItem label="Bairro" value={property.neighborhood} />
                  </Grid>
                  <Grid item xs={12} md={6}>
                    <InfoItem label="Rua" value={property.street} />
                  </Grid>
                  <Grid item xs={12} md={6}>
                    <InfoItem label="Número" value={property.number} />
                  </Grid>
                  <Grid item xs={12} md={6}>
                    <InfoItem label="Complemento" value={complementText} />
                  </Grid>
                  <Grid item xs={12} md={6}>
                    <InfoItem
                      label="Índice cadastral"
                      value={property.cadastralIndex ?? 'Não informado'}
                    />
                  </Grid>
                </Grid>

                <Divider sx={{ my: 3 }} />

                <Stack direction="row" spacing={1} flexWrap="wrap" useFlexGap>
                  <Chip
                    label={property.isActive ? 'Ativo' : 'Inativo'}
                    color={property.isActive ? 'success' : 'default'}
                  />
                  <Chip
                    variant="outlined"
                    label={`Cadastrado em ${formatDateTime(property.createdAt)}`}
                  />
                  {property.updatedAt ? (
                    <Chip
                      variant="outlined"
                      label={`Atualizado em ${formatDateTime(property.updatedAt)}`}
                    />
                  ) : null}
                </Stack>
              </CardContent>
            </Card>

            <Stack direction={{ xs: 'column', sm: 'row' }} spacing={2}>
              <Button
                component={RouterLink}
                to={`/imoveis/${property.id}/historico`}
                variant="outlined"
                startIcon={<HistoryEduIcon />}
              >
                Histórico completo
              </Button>
              <Button
                component={RouterLink}
                to={`/imoveis/${property.id}/historico/novo`}
                variant="contained"
              >
                Registrar evento
              </Button>
            </Stack>
          </Stack>
        );
      }}
    </QueryState>
  );
}

function InfoItem({ label, value }: { label: string; value: string }) {
  return (
    <Box>
      <Typography variant="caption" color="text.secondary" display="block">
        {label}
      </Typography>
      <Typography variant="body1" fontWeight={600}>
        {value}
      </Typography>
    </Box>
  );
}
