import React, { useEffect, useState } from 'react';
import { axiosInstance } from '../axiosConfig';
import { useNavigate } from 'react-router-dom';
import Layout from '../components/Layout';

interface ProtectedData {
    message: string;
}

const Dashboard: React.FC = () => {
    const [data, setData] = useState<ProtectedData | null>(null);
    const navigate = useNavigate();

    useEffect(() => {
        axiosInstance.get<ProtectedData>('/protecteddata')
            .then((response) => setData(response.data))
            .catch((error) => console.error('Error fetching protected data:', error));
    }, []);

    const handleLogout = () => {
        localStorage.removeItem('token');
        navigate('/login');
    };

    return (
        <Layout>
            <section className="bg-gray-50 dark:bg-gray-900 min-h-screen">
                <div className="flex flex-col items-center justify-center px-6 py-8 mx-auto md:h-screen lg:py-0">
                    <div className="w-full bg-white rounded-lg shadow dark:border md:mt-0 sm:max-w-md xl:p-0 dark:bg-gray-800 dark:border-gray-700">
                        <div className="p-6 space-y-4 md:space-y-6 sm:p-8">
                            <h1 className="text-xl font-bold leading-tight tracking-tight text-gray-900 md:text-2xl dark:text-white">
                                Welcome to the Dashboard
                            </h1>

                            <div className="bg-gray-100 p-4 rounded-lg shadow-md">
                                <h3 className="font-semibold text-gray-900 dark:text-white">Protected Data:</h3>
                                <pre className="bg-gray-800 text-white p-4 rounded-lg mt-2">
                                    {JSON.stringify(data, null, 2)}
                                </pre>
                            </div>

                            <div className="mt-4">
                                <button
                                    onClick={handleLogout}
                                    type="button"
                                    className="w-full bg-red-600 text-white font-semibold rounded-lg px-5 py-2.5 hover:bg-red-700 focus:outline-none focus:ring-2 focus:ring-red-500"
                                >
                                    Log Out
                                </button>
                            </div>
                        </div>
                    </div>
                </div>
            </section>
        </Layout>
    );
};

export default Dashboard;
