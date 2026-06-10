import AddIcon from '@mui/icons-material/Add';
import EditIcon from '@mui/icons-material/Edit';
import InfoOutlinedIcon from '@mui/icons-material/InfoOutlined';
import ToggleOffIcon from '@mui/icons-material/ToggleOff';
import ToggleOnIcon from '@mui/icons-material/ToggleOn';
import {
  Alert,
  Box,
  Button,
  Dialog,
  DialogActions,
  DialogContent,
  DialogContentText,
  DialogTitle,
  FormControl,
  IconButton,
  InputLabel,
  MenuItem,
  Select,
  Stack,
  Table,
  TableBody,
  TableCell,
  TableContainer,
  TableHead,
  TableRow,
  TextField,
  Tooltip,
} from '@mui/material';
import { useMemo, useState } from 'react';
import { Link as RouterLink } from 'react-router-dom';
import { getApiErrorDetails } from '@/api/apiClient';
import { EmptyState } from '@/components/common/EmptyState';
import { ErrorAlert } from '@/components/common/ErrorAlert';
import { LoadingState } from '@/components/common/LoadingState';
import { PageHeader } from '@/components/common/PageHeader';
import { AppCard } from '@/components/ui/AppCard';
import {
  useActivateCity,
  useCities,
  useDeactivateCity,
  type CityStatusFilter,
} from '@/hooks/useCities';
import { StatusChip } from '@/components/ui/StatusChip';
import type { CityResponse } from '@/types/api';
function formatCityLabel(city: CityResponse): string {
  return `${city.name} — ${city.state}`;
}

export function CityListPage() {
  const [nameFilter, setNameFilter] = useState('');
  const [statusFilter, setStatusFilter] = useState<CityStatusFilter>('All');
  const [appliedFilters, setAppliedFilters] = useState<{ name?: string; status: CityStatusFilter }>({
    status: 'All',
  });
  const [cityToDeactivate, setCityToDeactivate] = useState<CityResponse | null>(null);
  const [feedback, setFeedback] = useState<string | null>(null);

  const citiesQuery = useCities(appliedFilters);
  const activateMutation = useActivateCity();
  const deactivateMutation = useDeactivateCity();

  const isMutating = activateMutation.isPending || deactivateMutation.isPending;
  const mutationError = activateMutation.error ?? deactivateMutation.error;

  const sortedCities = useMemo(
    () => [...(citiesQuery.data ?? [])].sort((a, b) => a.name.localeCompare(b.name, 'pt-BR')),
    [citiesQuery.data],
  );

  const handleSearch = () => {
    setAppliedFilters({
      name: nameFilter.trim() || undefined,
      status: statusFilter,
    });
    setFeedback(null);
  };

  const handleActivate = async (city: CityResponse) => {
    setFeedback(null);
    try {
      await activateMutation.mutateAsync(city.id);
      setFeedback('Cidade ativada com sucesso.');
    } catch {
      // erro exibido via ErrorAlert
    }
  };

  const handleConfirmDeactivate = async () => {
    if (!cityToDeactivate) {
      return;
    }

    setFeedback(null);
    try {
      await deactivateMutation.mutateAsync(cityToDeactivate.id);
      setFeedback('Cidade inativada com sucesso.');
      setCityToDeactivate(null);
    } catch {
      // erro exibido via ErrorAlert
    }
  };

  return (
    <Stack spacing={3}>
      <PageHeader
        title="Cidades"
        subtitle="Gerencie as cidades disponíveis para cadastro de imóveis."
        action={
          <Button
            component={RouterLink}
            to="/cidades/novo"
            variant="contained"
            startIcon={<AddIcon />}
          >
            Nova cidade
          </Button>
        }
      />

      {feedback ? (
        <Alert severity="success" sx={{ borderRadius: 2 }} onClose={() => setFeedback(null)}>
          {feedback}
        </Alert>
      ) : null}

      {mutationError ? (
        <ErrorAlert
          message={getApiErrorDetails(mutationError).message}
          errors={getApiErrorDetails(mutationError).errors}
        />
      ) : null}

      <AppCard noHover>
          <Stack
            direction={{ xs: 'column', md: 'row' }}
            spacing={2}
            alignItems={{ md: 'flex-end' }}
            sx={{ mb: 3 }}
          >
            <TextField
              label="Filtrar por nome"
              value={nameFilter}
              onChange={(event) => setNameFilter(event.target.value)}
              fullWidth
              placeholder="Ex.: Belo Horizonte"
            />
            <FormControl sx={{ minWidth: { md: 200 }, width: { xs: '100%', md: 'auto' } }}>
              <InputLabel id="city-status-filter-label">Status</InputLabel>
              <Select
                labelId="city-status-filter-label"
                label="Status"
                value={statusFilter}
                onChange={(event) => setStatusFilter(event.target.value as CityStatusFilter)}
              >
                <MenuItem value="Active">Ativas</MenuItem>
                <MenuItem value="Inactive">Inativas</MenuItem>
                <MenuItem value="All">Todas</MenuItem>
              </Select>
            </FormControl>
            <Button variant="contained" onClick={handleSearch} sx={{ minWidth: 120 }}>
              Filtrar
            </Button>
          </Stack>

          {citiesQuery.isLoading ? <LoadingState message="Carregando cidades..." /> : null}

          {citiesQuery.error ? (
            <ErrorAlert message={getApiErrorDetails(citiesQuery.error).message} />
          ) : null}

          {!citiesQuery.isLoading && !citiesQuery.error && sortedCities.length === 0 ? (
            <EmptyState
              title="Nenhuma cidade encontrada"
              description="Ajuste os filtros ou cadastre uma nova cidade."
            />
          ) : null}

          {sortedCities.length > 0 ? (
            <TableContainer>
              <Table size="medium">
                <TableHead>
                  <TableRow>
                    <TableCell>Nome</TableCell>
                    <TableCell>Estado</TableCell>
                    <TableCell>Status</TableCell>
                    <TableCell align="right">Ações</TableCell>
                  </TableRow>
                </TableHead>
                <TableBody>
                  {sortedCities.map((city) => (
                    <TableRow key={city.id} hover>
                      <TableCell>{city.name}</TableCell>
                      <TableCell>{city.state}</TableCell>
                      <TableCell>
                        <StatusChip
                          active={city.isActive}
                          label={city.isActive ? 'Ativa' : 'Inativa'}
                        />
                      </TableCell>
                      <TableCell align="right">
                        <Stack direction="row" spacing={0.5} justifyContent="flex-end">
                          <Tooltip title="Detalhes">
                            <IconButton
                              component={RouterLink}
                              to={`/cidades/detalhes/${city.id}`}
                              size="small"
                              aria-label={`Detalhes de ${formatCityLabel(city)}`}
                            >
                              <InfoOutlinedIcon fontSize="small" />
                            </IconButton>
                          </Tooltip>
                          <Tooltip title="Editar">
                            <IconButton
                              component={RouterLink}
                              to={`/cidades/editar/${city.id}`}
                              size="small"
                              aria-label={`Editar ${formatCityLabel(city)}`}
                            >
                              <EditIcon fontSize="small" />
                            </IconButton>
                          </Tooltip>
                          {city.isActive ? (
                            <Tooltip title="Inativar">
                              <span>
                                <IconButton
                                  size="small"
                                  aria-label={`Inativar ${formatCityLabel(city)}`}
                                  disabled={isMutating}
                                  onClick={() => setCityToDeactivate(city)}
                                >
                                  <ToggleOffIcon fontSize="small" />
                                </IconButton>
                              </span>
                            </Tooltip>
                          ) : (
                            <Tooltip title="Ativar">
                              <span>
                                <IconButton
                                  size="small"
                                  aria-label={`Ativar ${formatCityLabel(city)}`}
                                  disabled={isMutating}
                                  onClick={() => handleActivate(city)}
                                >
                                  <ToggleOnIcon fontSize="small" />
                                </IconButton>
                              </span>
                            </Tooltip>
                          )}
                        </Stack>
                      </TableCell>
                    </TableRow>
                  ))}
                </TableBody>
              </Table>
            </TableContainer>
          ) : null}
      </AppCard>

      <Dialog open={Boolean(cityToDeactivate)} onClose={() => setCityToDeactivate(null)}>
        <DialogTitle>Inativar cidade</DialogTitle>
        <DialogContent>
          <DialogContentText>
            Deseja inativar esta cidade? Ela não aparecerá mais em novos cadastros.
            {cityToDeactivate ? (
              <Box component="span" sx={{ display: 'block', mt: 1, fontWeight: 600 }}>
                {formatCityLabel(cityToDeactivate)}
              </Box>
            ) : null}
          </DialogContentText>
        </DialogContent>
        <DialogActions>
          <Button onClick={() => setCityToDeactivate(null)} disabled={deactivateMutation.isPending}>
            Cancelar
          </Button>
          <Button
            onClick={handleConfirmDeactivate}
            color="warning"
            variant="contained"
            disabled={deactivateMutation.isPending}
          >
            Inativar
          </Button>
        </DialogActions>
      </Dialog>
    </Stack>
  );
}
