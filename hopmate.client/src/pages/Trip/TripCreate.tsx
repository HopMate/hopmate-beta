/* eslint-disable @typescript-eslint/no-explicit-any */
import React, { useState } from 'react';
import { useNavigate } from 'react-router-dom';
import Layout from '../../components/Layout';
import { axiosInstance } from '../../axiosConfig';

const CreateTrip: React.FC = () => {
    const navigate = useNavigate();
    const driverId = localStorage.getItem('userId');
    const [form, setForm] = useState({
        departureTime: '',
        arrivalTime: '',
        startAddress: '',
        startPostalCode: '',
        endAddress: '',
        endPostalCode: '',
    });
    const [submitting, setSubmitting] = useState(false);
    const [error, setError] = useState('');

    const handleChange = (e: React.ChangeEvent<HTMLInputElement>) => {
        setForm({ ...form, [e.target.name]: e.target.value });
    };

    const handleSubmit = async (e: React.FormEvent) => {
        e.preventDefault();
        setSubmitting(true);
        setError('');

        try {
            await axiosInstance.post('/trip', {
                driverId,
                departureTime: form.departureTime,
                arrivalTime: form.arrivalTime,
                startLocation: {
                    address: form.startAddress,
                    postalCode: form.startPostalCode,
                },
                endLocation: {
                    address: form.endAddress,
                    postalCode: form.endPostalCode,
                },
            });

            navigate('/trips'); 
        } catch (err: any) {
            setError('Erro ao criar viagem.');
            console.error(err);
        } finally {
            setSubmitting(false);
        }
    };

    return (
        <Layout>
            <section className="p-6 max-w-3xl mx-auto">
                <h1 className="text-2xl font-bold mb-6 text-gray-800 dark:text-white">Criar Nova Viagem</h1>

                <form onSubmit={handleSubmit} className="space-y-4">
                    <div className="grid grid-cols-1 md:grid-cols-2 gap-4">
                        <div>
                            <label className="block text-gray-700 dark:text-white mb-1">Data de Partida</label>
                            <input
                                type="datetime-local"
                                name="departureTime"
                                required
                                value={form.departureTime}
                                onChange={handleChange}
                                className="w-full px-3 py-2 rounded border dark:bg-gray-800 dark:text-white"
                            />
                        </div>
                        <div>
                            <label className="block text-gray-700 dark:text-white mb-1">Data de Chegada</label>
                            <input
                                type="datetime-local"
                                name="arrivalTime"
                                required
                                value={form.arrivalTime}
                                onChange={handleChange}
                                className="w-full px-3 py-2 rounded border dark:bg-gray-800 dark:text-white"
                            />
                        </div>
                    </div>

                    <div>
                        <h2 className="text-lg font-semibold mb-2 text-gray-700 dark:text-white">Local de Partida</h2>
                        <input
                            type="text"
                            name="startAddress"
                            placeholder="Endereço"
                            required
                            value={form.startAddress}
                            onChange={handleChange}
                            className="w-full px-3 py-2 mb-2 rounded border dark:bg-gray-800 dark:text-white"
                        />
                        <input
                            type="text"
                            name="startPostalCode"
                            placeholder="Código Postal (1234-567)"
                            pattern="\d{4}-\d{3}"
                            required
                            value={form.startPostalCode}
                            onChange={handleChange}
                            className="w-full px-3 py-2 rounded border dark:bg-gray-800 dark:text-white"
                        />
                    </div>

                    <div>
                        <h2 className="text-lg font-semibold mb-2 text-gray-700 dark:text-white">Local de Chegada</h2>
                        <input
                            type="text"
                            name="endAddress"
                            placeholder="Endereço"
                            required
                            value={form.endAddress}
                            onChange={handleChange}
                            className="w-full px-3 py-2 mb-2 rounded border dark:bg-gray-800 dark:text-white"
                        />
                        <input
                            type="text"
                            name="endPostalCode"
                            placeholder="Código Postal (1234-567)"
                            pattern="\d{4}-\d{3}"
                            required
                            value={form.endPostalCode}
                            onChange={handleChange}
                            className="w-full px-3 py-2 rounded border dark:bg-gray-800 dark:text-white"
                        />
                    </div>

                    {error && <p className="text-red-600">{error}</p>}

                    <button
                        type="submit"
                        disabled={submitting}
                        className="bg-blue-600 hover:bg-blue-700 text-white px-6 py-2 rounded"
                    >
                        {submitting ? 'A Criar...' : 'Criar Viagem'}
                    </button>
                </form>
            </section>
        </Layout>
    );
};

export default CreateTrip;
