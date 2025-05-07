// src/pages/TripList.tsx
import React, { useEffect, useState } from 'react';
import { Link } from 'react-router-dom';
import { axiosInstance } from '../../axiosConfig';
import Layout from '../../components/Layout';

interface Trip {
    tripId: string;
    departureTime: string;
    arrivalTime: string;
    startLocation?: string;
    endLocation?: string;
    driverName?: string; // Apenas presente em trips como passageiro
}

const TripList: React.FC = () => {
    const [trips, setTrips] = useState<Trip[]>([]);
    const [loading, setLoading] = useState(true);
    const [view, setView] = useState<'driver' | 'passenger'>('driver');
    const userId = localStorage.getItem('userId');

    useEffect(() => {
        if (!userId) return;

        setLoading(true);

        const endpoint =
            view === 'driver'
                ? `/trip/user/${userId}/driver-trips`
                : `/trip/user/${userId}/passenger-trips`;

        axiosInstance
            .get(endpoint)
            .then((response) => {
                const data = response.data;
                const tripsArray = Array.isArray(data) ? data : data.$values; // <-- aqui está a magia
                setTrips(tripsArray || []);
            })
            .catch((error) => {
                console.error('Erro ao buscar viagens:', error);
                setTrips([]);
            })
            .finally(() => setLoading(false));
    }, [view, userId]);


    return (
        <Layout>
            <section className="p-6 max-w-5xl mx-auto">
                <h1 className="text-2xl font-bold mb-4 text-gray-800 dark:text-white">Minhas Viagens</h1>

                <div className="flex justify-between items-center mb-4">
                    <div className="space-x-4">
                        <button
                            onClick={() => setView('driver')}
                            className={`px-4 py-2 rounded ${view === 'driver'
                                ? 'bg-blue-600 text-white'
                                : 'bg-gray-300 dark:bg-gray-700 text-gray-800 dark:text-white'
                                }`}
                        >
                            Como Condutor
                        </button>
                        <button
                            onClick={() => setView('passenger')}
                            className={`px-4 py-2 rounded ${view === 'passenger'
                                ? 'bg-blue-600 text-white'
                                : 'bg-gray-300 dark:bg-gray-700 text-gray-800 dark:text-white'
                                }`}
                        >
                            Como Passageiro
                        </button>
                    </div>

                    {view === 'driver' && (
                        <Link
                            to="/trip/create"
                            className="bg-green-600 text-white px-4 py-2 rounded hover:bg-green-700"
                        >
                            Nova Viagem
                        </Link>
                    )}
                </div>

                {loading ? (
                    <p className="text-gray-600 dark:text-gray-300">A carregar viagens...</p>
                ) : trips.length === 0 ? (
                    <p className="text-gray-600 dark:text-gray-300">Nenhuma viagem encontrada.</p>
                ) : (
                    <div className="overflow-x-auto">
                        <table className="min-w-full bg-white dark:bg-gray-800 shadow rounded-lg">
                            <thead>
                                <tr className="bg-gray-100 dark:bg-gray-700 text-gray-700 dark:text-gray-200">
                                    <th className="px-4 py-2 text-left">Partida</th>
                                    <th className="px-4 py-2 text-left">Chegada</th>
                                    <th className="px-4 py-2 text-left">De</th>
                                    <th className="px-4 py-2 text-left">Para</th>
                                    {view === 'passenger' && (
                                        <th className="px-4 py-2 text-left">Condutor</th>
                                    )}
                                    <th className="px-4 py-2 text-left">Ações</th>
                                </tr>
                            </thead>
                            <tbody>
                                {trips.map((trip) => (
                                    <tr key={trip.TripId} className="border-t border-gray-200 dark:border-gray-700">
                                        <td className="px-4 py-2">
                                            {new Date(trip.DepartureTime).toLocaleString()}
                                        </td>
                                        <td className="px-4 py-2">
                                            {new Date(trip.ArrivalTime).toLocaleString()}
                                        </td>
                                        <td className="px-4 py-2">{trip.StartLocation || 'N/A'}</td>
                                        <td className="px-4 py-2">{trip.EndLocation || 'N/A'}</td>
                                        {view === 'passenger' && (
                                            <td className="px-4 py-2">{trip.DriverName || '—'}</td>
                                        )}
                                        <td className="px-4 py-2 space-x-2">
                                            <Link
                                                to={`/trip/details/${trip.TripId}`}
                                                className="text-blue-600 hover:underline"
                                            >
                                                Detalhes
                                            </Link>
                                            {view === 'driver' && (
                                                <Link
                                                    to={`/trip/cancel/${trip.TripId}`}
                                                    className="text-red-600 hover:underline"
                                                >
                                                    Cancelar
                                                </Link>
                                            )}
                                        </td>
                                    </tr>
                                ))}
                            </tbody>
                        </table>
                    </div>
                )}
            </section>
        </Layout>
    );
}

export default TripList;