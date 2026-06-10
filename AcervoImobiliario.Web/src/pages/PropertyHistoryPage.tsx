import AddIcon from '@mui/icons-material/Add';
import ArrowBackIcon from '@mui/icons-material/ArrowBack';
import SortIcon from '@mui/icons-material/Sort';
import {
  Button,
  Stack,
  ToggleButton,
  ToggleButtonGroup,
  Typography,
} from '@mui/material';
import { useState } from 'react';
import { Link as RouterLink, useParams } from 'react-router-dom';
import { getApiErrorDetails } from '@/api/apiClient';
import { EmptyState } from '@/components/common/EmptyState';
import { ErrorAlert } from '@/components/common/ErrorAlert';
import { LoadingState } from '@/components/common/LoadingState';
import { PageHeader } from '@/components/common/PageHeader';
import { HistoryTimeline } from '@/components/history/HistoryTimeline';
import { PropertySummaryCard } from '@/components/property/PropertySummaryCard';
import { AppCard } from '@/components/ui/AppCard';
import { useProperty } from '@/hooks/useProperties';
import { usePropertyHistories } from '@/hooks/usePropertyHistories';
import type { HistorySortDirection } from '@/types/api';

export function PropertyHistoryPage() {
  const { id = '' } = useParams();
  const [sortDirection, setSortDirection] = useState<HistorySortDirection>('desc');

  const propertyQuery = useProperty(id);
  const historiesQuery = usePropertyHistories(id, sortDirection);

  const isLoading = propertyQuery.isLoading || historiesQuery.isLoading;
  const error = propertyQuery.error ?? historiesQuery.error;

  if (isLoading) {
    return <LoadingState message="Carregando histórico do imóvel..." />;
  }

  if (error) {
    const details = getApiErrorDetails(error);
    return <ErrorAlert message={details.message} errors={details.errors} />;
  }

  if (!propertyQuery.data) {
    return (
      <EmptyState
        title="Imóvel não encontrado"
        description="Não foi possível carregar o imóvel para exibir o histórico."
      />
    );
  }

  const property = propertyQuery.data;
  const histories = historiesQuery.data ?? [];

  return (
    <Stack spacing={3}>
      <PageHeader
        title="Histórico do imóvel"
        subtitle="Linha do tempo de eventos registrados para este endereço."
        action={
          <Stack direction={{ xs: 'column', sm: 'row' }} spacing={1}>
            <Button
              component={RouterLink}
              to={`/imoveis/${id}`}
              variant="outlined"
              color="primary"
              startIcon={<ArrowBackIcon />}
            >
              Voltar ao imóvel
            </Button>
            <Button
              component={RouterLink}
              to={`/imoveis/${id}/historico/novo`}
              variant="contained"
              color="primary"
              startIcon={<AddIcon />}
            >
              Adicionar histórico
            </Button>
          </Stack>
        }
      />

      <PropertySummaryCard property={property} />

      <AppCard noHover>
          <Stack
            direction={{ xs: 'column', sm: 'row' }}
            justifyContent="space-between"
            alignItems={{ xs: 'stretch', sm: 'center' }}
            spacing={2}
            sx={{ mb: 3 }}
          >
            <div>
              <Typography variant="h6">Linha do tempo</Typography>
              <Typography variant="body2" color="text.secondary">
                {histories.length}{' '}
                {histories.length === 1 ? 'evento registrado' : 'eventos registrados'}
              </Typography>
            </div>

            <ToggleButtonGroup
              size="small"
              exclusive
              value={sortDirection}
              onChange={(_, value: HistorySortDirection | null) => {
                if (value) setSortDirection(value);
              }}
              aria-label="Ordenação do histórico"
            >
              <ToggleButton value="desc">
                <SortIcon sx={{ mr: 0.75, fontSize: 18 }} />
                Mais recente
              </ToggleButton>
              <ToggleButton value="asc">
                <SortIcon sx={{ mr: 0.75, fontSize: 18, transform: 'scaleY(-1)' }} />
                Mais antigo
              </ToggleButton>
            </ToggleButtonGroup>
          </Stack>

          {histories.length > 0 ? (
            <HistoryTimeline histories={histories} />
          ) : (
            <EmptyState
              title="Nenhum evento registrado"
              description="Este imóvel ainda não possui histórico. Use o botão Adicionar histórico para registrar o primeiro evento."
            />
          )}
      </AppCard>
    </Stack>
  );
}
