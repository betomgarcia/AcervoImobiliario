import EditIcon from '@mui/icons-material/Edit';
import LocationCityIcon from '@mui/icons-material/LocationCity';
import { Alert, Button, Grid, Stack, Typography } from '@mui/material';
import { useEffect, useState } from 'react';
import { Link as RouterLink, useLocation, useParams } from 'react-router-dom';
import { QueryState } from '@/components/common/QueryState';
import { PageHeader } from '@/components/common/PageHeader';
import { AppCard } from '@/components/ui/AppCard';
import { InfoField } from '@/components/ui/InfoField';
import { StatusChip } from '@/components/ui/StatusChip';
import { useCity } from '@/hooks/useCities';
import { formatDateTime } from '@/utils/format';

export function CityDetailPage() {
  const { id = '' } = useParams();
  const location = useLocation();
  const cityQuery = useCity(id);
  const [flashMessage, setFlashMessage] = useState<string | null>(null);

  useEffect(() => {
    const state = location.state as { created?: boolean; updated?: boolean } | null;
    if (state?.created) {
      setFlashMessage('Cidade cadastrada com sucesso.');
    } else if (state?.updated) {
      setFlashMessage('Cidade atualizada com sucesso.');
    }
  }, [location.state]);

  return (
    <QueryState
      isLoading={cityQuery.isLoading}
      error={cityQuery.error}
      data={cityQuery.data}
      loadingMessage="Carregando cidade..."
      emptyTitle="Cidade não encontrada"
      emptyDescription="A cidade solicitada não existe."
      isEmpty={(city) => !city}
    >
      {(city) => (
        <Stack spacing={3}>
          <PageHeader
            title="Detalhes da cidade"
            subtitle={`${city.name} — ${city.state}`}
            action={
              <Stack direction={{ xs: 'column', sm: 'row' }} spacing={1}>
                <Button component={RouterLink} to="/cidades" variant="outlined" color="primary">
                  Voltar
                </Button>
                <Button
                  component={RouterLink}
                  to={`/cidades/editar/${city.id}`}
                  variant="contained"
                  color="primary"
                  startIcon={<EditIcon />}
                >
                  Editar
                </Button>
              </Stack>
            }
          />

          {flashMessage ? (
            <Alert severity="success" sx={{ borderRadius: 2 }} onClose={() => setFlashMessage(null)}>
              {flashMessage}
            </Alert>
          ) : null}

          <AppCard noHover>
            <Stack direction="row" spacing={1} alignItems="center" sx={{ mb: 3 }}>
              <LocationCityIcon color="primary" />
              <Typography variant="h5">Informações</Typography>
            </Stack>

            <Grid container spacing={2}>
              <Grid item xs={12} md={6}>
                <InfoField label="Nome" value={city.name} />
              </Grid>
              <Grid item xs={12} md={6}>
                <InfoField label="Estado" value={city.state} />
              </Grid>
              <Grid item xs={12} md={6}>
                <Stack spacing={0.5}>
                  <Typography variant="overline" color="text.secondary">
                    Status
                  </Typography>
                  <StatusChip
                    active={city.isActive}
                    label={city.isActive ? 'Ativa' : 'Inativa'}
                    sx={{ alignSelf: 'flex-start' }}
                  />
                </Stack>
              </Grid>
              <Grid item xs={12} md={6}>
                <InfoField label="Cadastrada em" value={formatDateTime(city.createdAt)} />
              </Grid>
              {city.updatedAt ? (
                <Grid item xs={12} md={6}>
                  <InfoField label="Última alteração" value={formatDateTime(city.updatedAt)} />
                </Grid>
              ) : null}
            </Grid>
          </AppCard>
        </Stack>
      )}
    </QueryState>
  );
}
