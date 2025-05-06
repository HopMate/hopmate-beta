/* -*- coding: utf-8 -*- */
import React, { useEffect, useState } from 'react';
import { axiosInstance } from '../axiosConfig';
import Layout from '../components/Layout';

interface UserInfo {
    username: string;
    email?: string;
}

const Dashboard: React.FC = () => {
    const [user, setUser] = useState<UserInfo | null>(null);

    useEffect(() => {
        axiosInstance.get('/dashboard/userinfo')
            .then((response) => setUser(response.data))
            .catch((error) => console.error('Erro ao buscar dados protegidos:', error));
    }, []);

    return (
        <Layout>
            <main className="p-6 flex flex-col items-center justify-center text-center">
                <h1 className="text-3xl font-bold text-gray-800 dark:text-white mb-4">Dashboard do Projeto</h1>
                <p className="text-lg text-gray-600 dark:text-gray-300 max-w-xl">
                    Esta é a interface principal do projeto onde podes navegar pelas APIs, ver dados protegidos e interagir com as funcionalidades do sistema.
                </p>

                {/* Dados do utilizador */}
                {user && (
                    <div className="mt-6 bg-gray-100 dark:bg-gray-800 p-4 rounded shadow w-full max-w-2xl">
                        <h2 className="text-lg font-semibold text-gray-700 dark:text-white mb-2">Informação do Utilizador</h2>
                        <pre className="bg-gray-800 text-white p-4 rounded-lg text-left overflow-x-auto">
                            {JSON.stringify(user, null, 2)}
                        </pre>
                    </div>
                )}
            </main>
        </Layout>
    );
};

export default Dashboard;
