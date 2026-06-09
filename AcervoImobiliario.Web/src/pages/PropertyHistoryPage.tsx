import AddIcon from '@mui/icons-material/Add';
import ArrowBackIcon from '@mui/icons-material/ArrowBack';
import SortIcon from '@mui/icons-material/Sort';
import {
  Box,
  Button,
  Card,
  CardContent,
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
import { HistoryTimeline } from '@/components/history/HistoryTimeline';
import { PropertySummaryCard } from '@/components/property/PropertySummaryCard';
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
      <Stack
        direction={{ xs: 'column', md: 'row' }}
        justifyContent="space-between"
        alignItems={{ xs: 'stretch', md: 'flex-start' }}
        spacing={2}
      >
        <Box>
          <Typography variant="h4" color="primary.dark" fontWeight={700}>
            Histórico do imóvel
          </Typography>
          <Typography variant="body1" color="text.secondary" sx={{ mt: 0.5 }}>
            Linha do tempo de eventos registrados para este endereço.
          </Typography>
        </Box>

        <Stack direction={{ xs: 'column', sm: 'row' }} spacing={1}>
          <Button
            component={RouterLink}
            to={`/imoveis/${id}`}
            variant="outlined"
            startIcon={<ArrowBackIcon />}
          >
            Voltar ao imóvel
          </Button>
          <Button
            component={RouterLink}
            to={`/imoveis/${id}/historico/novo`}
            variant="contained"
            startIcon={<AddIcon />}
          >
            Adicionar histórico
          </Button>
        </Stack>
      </Stack>

      <PropertySummaryCard property={property} />

      <Card>
        <CardContent sx={{ p: { xs: 2, md: 3 } }}>
          <Stack
            direction={{ xs: 'column', sm: 'row' }}
            justifyContent="space-between"
            alignItems={{ xs: 'stretch', sm: 'center' }}
            spacing={2}
            sx={{ mb: 3 }}
          >
            <Box>
              <Typography variant="h6" fontWeight={700}>
                Linha do tempo
              </Typography>
              <Typography variant="body2" color="text.secondary">
                {histories.length}{' '}
                {histories.length === 1 ? 'evento registrado' : 'eventos registrados'}
              </Typography>
            </Box>

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
        </CardContent>
      </Card>
    </Stack>
  );
}
