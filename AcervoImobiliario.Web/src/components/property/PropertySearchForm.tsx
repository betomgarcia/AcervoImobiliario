import SearchIcon from '@mui/icons-material/Search';
import {
  Autocomplete,
  Box,
  Button,
  Card,
  CardContent,
  Divider,
  FormControl,
  Grid,
  InputLabel,
  MenuItem,
  Select,
  Stack,
  Step,
  StepLabel,
  Stepper,
  TextField,
  Typography,
} from '@mui/material';
import { useMemo, useState } from 'react';
import { useActiveCities, useSearchCities } from '@/hooks/useCities';
import { useDebouncedValue } from '@/hooks/useDebouncedValue';
import {
  useSearchNeighborhoods,
  useSearchNumbers,
  useSearchStreets,
} from '@/hooks/useProperties';
import { ComplementType, type CityResponse } from '@/types/api';
import { complementTypeLabels } from '@/utils/labels';

export type PropertySearchMode = 'address' | 'cadastral';

export interface PropertySearchFilters {
  mode: PropertySearchMode;
  cityId?: string;
  cityName?: string;
  neighborhood?: string;
  street?: string;
  number?: string;
  complementType?: ComplementType;
  complementValue?: string;
  cadastralIndex?: string;
}

interface PropertySearchFormProps {
  onSearch: (filters: PropertySearchFilters) => void;
  isSearching?: boolean;
}

const complementOptions = Object.entries(complementTypeLabels).map(([value, label]) => ({
  value: Number(value) as ComplementType,
  label,
}));

const addressSteps = ['Cidade', 'Bairro', 'Rua', 'Número', 'Complemento'];

function getActiveStep(
  city: CityResponse | null,
  neighborhood: string,
  street: string,
  number: string,
): number {
  if (!city) return 0;
  if (!neighborhood.trim()) return 1;
  if (!street.trim()) return 2;
  if (!number.trim()) return 3;
  return 4;
}

export function PropertySearchForm({ onSearch, isSearching }: PropertySearchFormProps) {
  const [city, setCity] = useState<CityResponse | null>(null);
  const [cityInput, setCityInput] = useState('');
  const [neighborhood, setNeighborhood] = useState('');
  const [neighborhoodInput, setNeighborhoodInput] = useState('');
  const [street, setStreet] = useState('');
  const [streetInput, setStreetInput] = useState('');
  const [number, setNumber] = useState('');
  const [numberInput, setNumberInput] = useState('');
  const [complementType, setComplementType] = useState<ComplementType | ''>('');
  const [complementValue, setComplementValue] = useState('');
  const [cadastralIndex, setCadastralIndex] = useState('');

  const isCadastralMode = cadastralIndex.trim().length > 0;

  const debouncedCityInput = useDebouncedValue(cityInput);
  const debouncedNeighborhoodInput = useDebouncedValue(neighborhoodInput);
  const debouncedStreetInput = useDebouncedValue(streetInput);
  const debouncedNumberInput = useDebouncedValue(numberInput);

  const { data: allCities = [] } = useActiveCities();
  const { data: searchedCities = [] } = useSearchCities(debouncedCityInput);

  const cityOptions = useMemo(() => {
    if (debouncedCityInput.trim().length >= 2) {
      return searchedCities;
    }
    return allCities;
  }, [allCities, debouncedCityInput, searchedCities]);

  const { data: neighborhoodOptions = [] } = useSearchNeighborhoods(
    city?.id ?? '',
    debouncedNeighborhoodInput,
  );

  const { data: streetOptions = [] } = useSearchStreets(
    city?.id ?? '',
    neighborhood,
    debouncedStreetInput,
  );

  const { data: numberOptions = [] } = useSearchNumbers(
    city?.id ?? '',
    neighborhood,
    street,
    debouncedNumberInput,
  );

  const activeStep = getActiveStep(city, neighborhood, street, number);

  const resetAddressFields = () => {
    setCity(null);
    setCityInput('');
    setNeighborhood('');
    setNeighborhoodInput('');
    setStreet('');
    setStreetInput('');
    setNumber('');
    setNumberInput('');
    setComplementType('');
    setComplementValue('');
  };

  const resetAfterCity = () => {
    setNeighborhood('');
    setNeighborhoodInput('');
    setStreet('');
    setStreetInput('');
    setNumber('');
    setNumberInput('');
    setComplementType('');
    setComplementValue('');
  };

  const resetAfterNeighborhood = () => {
    setStreet('');
    setStreetInput('');
    setNumber('');
    setNumberInput('');
    setComplementType('');
    setComplementValue('');
  };

  const resetAfterStreet = () => {
    setNumber('');
    setNumberInput('');
    setComplementType('');
    setComplementValue('');
  };

  const resetAfterNumber = () => {
    setComplementType('');
    setComplementValue('');
  };

  const handleCadastralChange = (value: string) => {
    setCadastralIndex(value);
    if (value.trim()) {
      resetAddressFields();
    }
  };

  const handleCityChange = (value: CityResponse | null) => {
    setCity(value);
    setCadastralIndex('');
    resetAfterCity();
  };

  const handleAddressSubmit = (event: React.FormEvent) => {
    event.preventDefault();
    if (!city) return;

    if (
      complementType !== '' &&
      complementType !== ComplementType.None &&
      !complementValue.trim()
    ) {
      return;
    }

    onSearch({
      mode: 'address',
      cityId: city.id,
      cityName: city.name,
      neighborhood: neighborhood || undefined,
      street: street || undefined,
      number: number || undefined,
      complementType: complementType === '' ? undefined : complementType,
      complementValue: complementValue || undefined,
    });
  };

  const handleCadastralSubmit = (event: React.FormEvent) => {
    event.preventDefault();
    const index = cadastralIndex.trim();
    if (!index) return;

    onSearch({
      mode: 'cadastral',
      cadastralIndex: index,
    });
  };

  const canSearchByAddress = Boolean(city?.id);
  const canSearchByCadastral = cadastralIndex.trim().length > 0;
  const complementEnabled = Boolean(number.trim()) && !isCadastralMode;

  return (
    <Stack spacing={2}>
      <Card>
        <CardContent sx={{ p: { xs: 2, md: 3 } }}>
          <Typography variant="h6" sx={{ mb: 0.5 }}>
            Busca por endereço
          </Typography>
          <Typography variant="body2" color="text.secondary" sx={{ mb: 2 }}>
            Preencha os campos em sequência. Cada etapa libera a próxima com autocomplete da API.
          </Typography>

          {!isCadastralMode ? (
            <Stepper
              activeStep={activeStep}
              alternativeLabel
              sx={{ mb: 3, display: { xs: 'none', sm: 'flex' } }}
            >
              {addressSteps.map((label) => (
                <Step key={label}>
                  <StepLabel>{label}</StepLabel>
                </Step>
              ))}
            </Stepper>
          ) : null}

          <Box
            component="form"
            onSubmit={handleAddressSubmit}
            sx={{ opacity: isCadastralMode ? 0.55 : 1, pointerEvents: isCadastralMode ? 'none' : 'auto' }}
          >
            <Grid container spacing={2}>
              <Grid item xs={12} md={6}>
                <Autocomplete
                  options={cityOptions}
                  value={city}
                  onChange={(_, value) => handleCityChange(value)}
                  inputValue={cityInput}
                  onInputChange={(_, value) => setCityInput(value)}
                  disabled={isCadastralMode}
                  getOptionLabel={(option) => `${option.name} — ${option.state}`}
                  isOptionEqualToValue={(option, value) => option.id === value.id}
                  renderInput={(params) => (
                    <TextField
                      {...params}
                      label="Cidade"
                      placeholder="Digite ao menos 2 letras"
                      required={!isCadastralMode}
                    />
                  )}
                />
              </Grid>

              <Grid item xs={12} md={6}>
                <Autocomplete
                  freeSolo
                  options={neighborhoodOptions}
                  value={neighborhood}
                  onChange={(_, value) => {
                    setNeighborhood(typeof value === 'string' ? value : value ?? '');
                    resetAfterNeighborhood();
                  }}
                  inputValue={neighborhoodInput}
                  onInputChange={(_, value) => setNeighborhoodInput(value)}
                  disabled={!city || isCadastralMode}
                  renderInput={(params) => (
                    <TextField
                      {...params}
                      label="Bairro"
                      placeholder={city ? 'Mín. 2 caracteres' : 'Selecione a cidade primeiro'}
                      helperText={!city ? 'Habilitado após selecionar a cidade' : undefined}
                    />
                  )}
                />
              </Grid>

              <Grid item xs={12} md={6}>
                <Autocomplete
                  freeSolo
                  options={streetOptions}
                  value={street}
                  onChange={(_, value) => {
                    setStreet(typeof value === 'string' ? value : value ?? '');
                    resetAfterStreet();
                  }}
                  inputValue={streetInput}
                  onInputChange={(_, value) => setStreetInput(value)}
                  disabled={!city || !neighborhood.trim() || isCadastralMode}
                  renderInput={(params) => (
                    <TextField
                      {...params}
                      label="Rua"
                      placeholder={neighborhood ? 'Mín. 2 caracteres' : 'Informe o bairro primeiro'}
                      helperText={
                        city && !neighborhood.trim()
                          ? 'Habilitado após informar o bairro'
                          : undefined
                      }
                    />
                  )}
                />
              </Grid>

              <Grid item xs={12} md={6}>
                <Autocomplete
                  freeSolo
                  options={numberOptions}
                  value={number}
                  onChange={(_, value) => {
                    setNumber(typeof value === 'string' ? value : value ?? '');
                    resetAfterNumber();
                  }}
                  inputValue={numberInput}
                  onInputChange={(_, value) => setNumberInput(value)}
                  disabled={!city || !neighborhood.trim() || !street.trim() || isCadastralMode}
                  renderInput={(params) => (
                    <TextField
                      {...params}
                      label="Número"
                      placeholder={street ? 'Somente dígitos' : 'Informe a rua primeiro'}
                      helperText={
                        neighborhood && street && !number
                          ? 'Habilitado após informar a rua'
                          : undefined
                      }
                    />
                  )}
                />
              </Grid>

              <Grid item xs={12}>
                <Typography variant="subtitle2" color="text.secondary" sx={{ mb: 1 }}>
                  Complemento
                </Typography>
              </Grid>

              <Grid item xs={12} md={4}>
                <FormControl fullWidth disabled={!complementEnabled}>
                  <InputLabel>Tipo</InputLabel>
                  <Select
                    label="Tipo"
                    value={complementType}
                    onChange={(event) => {
                      const value = event.target.value as ComplementType | '';
                      setComplementType(value);
                      if (value === '' || value === ComplementType.None) {
                        setComplementValue('');
                      }
                    }}
                  >
                    <MenuItem value="">
                      <em>Não filtrar</em>
                    </MenuItem>
                    {complementOptions.map((option) => (
                      <MenuItem key={option.value} value={option.value}>
                        {option.label}
                      </MenuItem>
                    ))}
                  </Select>
                </FormControl>
              </Grid>

              <Grid item xs={12} md={8}>
                <TextField
                  fullWidth
                  label="Valor do complemento"
                  value={complementValue}
                  onChange={(event) => setComplementValue(event.target.value)}
                  disabled={
                    !complementEnabled ||
                    complementType === '' ||
                    complementType === ComplementType.None
                  }
                  helperText={
                    !number.trim()
                      ? 'Habilitado após informar o número'
                      : complementType !== '' &&
                          complementType !== ComplementType.None &&
                          !complementValue.trim()
                        ? 'Informe o valor quando selecionar um tipo de complemento'
                        : undefined
                  }
                  error={
                    complementType !== '' &&
                    complementType !== ComplementType.None &&
                    !complementValue.trim()
                  }
                />
              </Grid>
            </Grid>

            <Stack direction="row" justifyContent="flex-end" sx={{ mt: 3 }}>
              <Button
                type="submit"
                variant="contained"
                size="large"
                startIcon={<SearchIcon />}
                disabled={!canSearchByAddress || isSearching || isCadastralMode}
              >
                {isSearching ? 'Buscando...' : 'Buscar por endereço'}
              </Button>
            </Stack>
          </Box>
        </CardContent>
      </Card>

      <Card>
        <CardContent sx={{ p: { xs: 2, md: 3 } }}>
          <Typography variant="h6" sx={{ mb: 0.5 }}>
            Busca por índice cadastral
          </Typography>
          <Typography variant="body2" color="text.secondary" sx={{ mb: 2 }}>
            Informe o índice cadastral para localizar o imóvel diretamente, sem preencher o
            endereço.
          </Typography>

          <Box component="form" onSubmit={handleCadastralSubmit}>
            <TextField
              fullWidth
              label="Índice cadastral"
              value={cadastralIndex}
              onChange={(event) => handleCadastralChange(event.target.value)}
              placeholder="Ex.: IDX-12345"
              helperText="Ao digitar o índice, a busca por endereço é desativada automaticamente."
            />

            <Stack direction="row" justifyContent="flex-end" sx={{ mt: 3 }}>
              <Button
                type="submit"
                variant="contained"
                color="secondary"
                size="large"
                startIcon={<SearchIcon />}
                disabled={!canSearchByCadastral || isSearching}
              >
                {isSearching ? 'Buscando...' : 'Buscar por índice'}
              </Button>
            </Stack>
          </Box>
        </CardContent>
      </Card>

      {isCadastralMode ? (
        <Typography variant="caption" color="text.secondary" align="center">
          Modo ativo: busca direta por índice cadastral
        </Typography>
      ) : city ? (
        <Typography variant="caption" color="text.secondary" align="center">
          Modo ativo: busca progressiva por endereço
        </Typography>
      ) : (
        <Divider />
      )}
    </Stack>
  );
}
