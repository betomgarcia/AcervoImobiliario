import SearchIcon from '@mui/icons-material/Search';

import TagIcon from '@mui/icons-material/Tag';

import {

  Autocomplete,

  Box,

  Button,

  Chip,

  Grid,

  Stack,

  Step,

  StepLabel,

  Stepper,

  TextField,

  Typography,

} from '@mui/material';

import { useState } from 'react';

import { CityAutocomplete } from '@/components/property/CityAutocomplete';

import { AppCard } from '@/components/ui/AppCard';

import { SectionHeader } from '@/components/ui/SectionHeader';

import { useDebouncedValue } from '@/hooks/useDebouncedValue';

import {

  useSearchNeighborhoods,

  useSearchNumbers,

  useSearchStreets,

} from '@/hooks/useProperties';

import type { CityResponse } from '@/types/api';



export type PropertySearchMode = 'address' | 'cadastral';



export interface PropertySearchFilters {

  mode: PropertySearchMode;

  cityId?: string;

  cityName?: string;

  neighborhood?: string;

  street?: string;

  number?: string;

  complement?: string;

  cadastralIndex?: string;

}



interface PropertySearchFormProps {

  onSearch: (filters: PropertySearchFilters) => void;

  isSearching?: boolean;

}



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

  const [neighborhood, setNeighborhood] = useState('');

  const [neighborhoodInput, setNeighborhoodInput] = useState('');

  const [street, setStreet] = useState('');

  const [streetInput, setStreetInput] = useState('');

  const [number, setNumber] = useState('');

  const [numberInput, setNumberInput] = useState('');

  const [complement, setComplement] = useState('');

  const [cadastralIndex, setCadastralIndex] = useState('');



  const isCadastralMode = cadastralIndex.trim().length > 0;



  const debouncedNeighborhoodInput = useDebouncedValue(neighborhoodInput);

  const debouncedStreetInput = useDebouncedValue(streetInput);

  const debouncedNumberInput = useDebouncedValue(numberInput);



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

    setNeighborhood('');

    setNeighborhoodInput('');

    setStreet('');

    setStreetInput('');

    setNumber('');

    setNumberInput('');

    setComplement('');

  };



  const resetAfterCity = () => {

    setNeighborhood('');

    setNeighborhoodInput('');

    setStreet('');

    setStreetInput('');

    setNumber('');

    setNumberInput('');

    setComplement('');

  };



  const resetAfterNeighborhood = () => {

    setStreet('');

    setStreetInput('');

    setNumber('');

    setNumberInput('');

    setComplement('');

  };



  const resetAfterStreet = () => {

    setNumber('');

    setNumberInput('');

    setComplement('');

  };



  const resetAfterNumber = () => {

    setComplement('');

  };



  const handleCadastralChange = (value: string) => {

    setCadastralIndex(value);

    if (value.trim()) {

      resetAddressFields();

    }

  };



  const handleCityChange = (_cityId: string, selectedCity: CityResponse | null) => {

    setCity(selectedCity);

    setCadastralIndex('');

    resetAfterCity();

  };



  const handleAddressSubmit = (event: React.FormEvent) => {

    event.preventDefault();

    if (!city) return;



    onSearch({

      mode: 'address',

      cityId: city.id,

      cityName: city.name,

      neighborhood: neighborhood || undefined,

      street: street || undefined,

      number: number || undefined,

      complement: complement.trim() || undefined,

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

    <Stack spacing={2.5}>

      <AppCard noHover>

        <SectionHeader

          title="Busca por endereço"

          description="Preencha os campos em sequência. Cada etapa libera a próxima com sugestões automáticas."

        />



        {!isCadastralMode ? (

          <Stepper

            activeStep={activeStep}

            alternativeLabel

            sx={{ mb: 3, display: { xs: 'none', md: 'flex' } }}

          >

            {addressSteps.map((label) => (

              <Step key={label}>

                <StepLabel>{label}</StepLabel>

              </Step>

            ))}

          </Stepper>

        ) : null}



        {!isCadastralMode ? (

          <Chip

            size="small"

            label={`Etapa ${activeStep + 1} de ${addressSteps.length}: ${addressSteps[activeStep]}`}

            color="primary"

            variant="outlined"

            sx={{ mb: 2, display: { xs: 'inline-flex', md: 'none' } }}

          />

        ) : null}



        <Box

          component="form"

          onSubmit={handleAddressSubmit}

          sx={{ opacity: isCadastralMode ? 0.5 : 1, pointerEvents: isCadastralMode ? 'none' : 'auto' }}

        >

          <Grid container spacing={2}>

            <Grid item xs={12} md={6}>

              <CityAutocomplete

                value={city?.id ?? ''}

                onChange={handleCityChange}

                disabled={isCadastralMode}

                placeholder="Digite ao menos 2 letras"

                required={!isCadastralMode}

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

              <TextField

                fullWidth

                label="Complemento (opcional)"

                value={complement}

                onChange={(event) => setComplement(event.target.value)}

                disabled={!complementEnabled}

                placeholder="Ex.: Apto 303 Bloco A"

                helperText={

                  !number.trim()

                    ? 'Habilitado após informar o número'

                    : 'Busca considera variações de escrita (ex.: apto = apartamento)'

                }

              />

            </Grid>

          </Grid>



          <Stack direction="row" justifyContent="flex-end" sx={{ mt: 3 }}>

            <Button

              type="submit"

              variant="contained"

              color="primary"

              size="large"

              startIcon={<SearchIcon />}

              disabled={!canSearchByAddress || isSearching || isCadastralMode}

            >

              {isSearching ? 'Buscando...' : 'Buscar por endereço'}

            </Button>

          </Stack>

        </Box>

      </AppCard>



      <AppCard noHover>

        <SectionHeader

          title="Busca por índice cadastral"

          description="Informe o índice cadastral para localizar o imóvel diretamente, sem preencher o endereço."

        />



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

              variant="outlined"

              color="primary"

              size="large"

              startIcon={<TagIcon />}

              disabled={!canSearchByCadastral || isSearching}

            >

              {isSearching ? 'Buscando...' : 'Buscar por índice'}

            </Button>

          </Stack>

        </Box>

      </AppCard>



      {isCadastralMode ? (

        <Typography variant="caption" color="text.secondary" align="center" display="block">

          Modo ativo: busca direta por índice cadastral

        </Typography>

      ) : city ? (

        <Typography variant="caption" color="text.secondary" align="center" display="block">

          Modo ativo: busca progressiva por endereço

        </Typography>

      ) : null}

    </Stack>

  );

}


