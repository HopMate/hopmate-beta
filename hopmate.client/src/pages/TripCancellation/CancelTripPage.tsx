import React, { useState } from 'react';
import { useParams } from 'react-router-dom';
import { axiosInstance } from '../../axiosConfig';
import Layout from '../../components/Layout';
import axios from 'axios';

interface TripSimilarityRequestDto {
    dateDeparture: string;
    dateArrival: string;
    postalOrigin: string;
    postalDestination: string;
}

interface TripDto {
    id: string;
    dtDeparture: string;
    dtArrival: string;
}

const CancelTripPage: React.FC = () => {
    const { id } = useParams();
    const [step, setStep] = useState<'initial' | 'confirmSearch' | 'showResult'>('initial');
    const [similarityDto, setSimilarityDto] = useState<TripSimilarityRequestDto | null>(null);
    const [similarTrip, setSimilarTrip] = useState<TripDto | null>(null);
    const [isCooldown, setIsCooldown] = useState(false);

    const handleCancelTrip = async () => {
        if (isCooldown) return;
        setIsCooldown(true);
        setTimeout(() => setIsCooldown(false), 5000);

        try {
            const response = await axiosInstance.post(`/trip/cancel/${id}`);
            if (typeof response.data === 'string') {
                alert(response.data);
            } else {
                setSimilarityDto(response.data);
                setStep('confirmSearch');
            }
        } catch (error: unknown) { 
            if (axios.isAxiosError(error) && error.response) {
                alert(error.response.data || 'Erro ao cancelar viagem');
            } else {
                alert('Erro inesperado.');
            }
        }
    };

    const handleSearchSimilar = async () => {
        if (!similarityDto) return;
        try {
            const response = await axiosInstance.post('/trip/searchsimilar', similarityDto);
            setSimilarTrip(response.data);
            setStep('showResult');
        } catch {
            alert('Erro ao procurar viagem semelhante');
        }
    };

    const resetFlow = () => {
        setStep('initial');
        setSimilarityDto(null);
        setSimilarTrip(null);
    };

    return (
        <Layout>
            <section className="bg-gray-50 dark:bg-gray-900 min-h-screen flex items-center justify-center px-4 py-8">
                <div className="bg-white dark:bg-gray-800 rounded-lg shadow p-6 sm:p-8 w-full max-w-md">
                    {step === 'initial' && (
                        <>
                            <h2 className="text-2xl font-bold text-gray-900 dark:text-white mb-6">
                                Cancelar Viagem
                            </h2>
                            <button
                                onClick={handleCancelTrip}
                                disabled={isCooldown}
                                className={`w-full px-4 py-2 rounded text-white font-semibold transition ${isCooldown ? 'bg-gray-400 cursor-not-allowed' : 'bg-red-600 hover:bg-red-700'
                                    }`}
                            >
                                {isCooldown ? 'Aguarde...' : 'Confirmar Cancelamento'}
                            </button>
                        </>
                    )}

                    {step === 'confirmSearch' && (
                        <>
                            <p className="text-gray-700 dark:text-gray-300 mb-4">
                                Viagem cancelada com sucesso. Deseja procurar uma semelhante?
                            </p>
                            <div className="flex flex-col sm:flex-row justify-between gap-2">
                                <button
                                    onClick={handleSearchSimilar}
                                    className="bg-blue-600 text-white px-4 py-2 rounded hover:bg-blue-700"
                                >
                                    Sim
                                </button>
                                <button
                                    onClick={resetFlow}
                                    className="bg-gray-500 text-white px-4 py-2 rounded hover:bg-gray-600"
                                >
                                    Não
                                </button>
                            </div>
                        </>
                    )}

                    {step === 'showResult' && (
                        <>
                            <h3 className="text-lg font-semibold text-gray-900 dark:text-white mb-2">
                                Viagem Semelhante Encontrada
                            </h3>
                            {similarTrip ? (
                                <div className="bg-gray-100 dark:bg-gray-700 p-4 rounded shadow text-sm text-gray-900 dark:text-gray-100">
                                    <p><strong>ID:</strong> {similarTrip.id}</p>
                                    <p><strong>Partida:</strong> {new Date(similarTrip.dtDeparture).toLocaleString()}</p>
                                    <p><strong>Chegada:</strong> {new Date(similarTrip.dtArrival).toLocaleString()}</p>
                                </div>
                            ) : (
                                <p className="text-gray-700 dark:text-gray-300">Nenhuma viagem semelhante foi encontrada.</p>
                            )}
                            <button
                                onClick={resetFlow}
                                className="mt-4 w-full bg-gray-500 text-white px-4 py-2 rounded hover:bg-gray-600"
                            >
                                Voltar
                            </button>
                        </>
                    )}
                </div>
            </section>
        </Layout>
    );
};

export default CancelTripPage;
