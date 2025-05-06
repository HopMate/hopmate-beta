import React, { useEffect, useState } from 'react';
import { axiosInstance } from '../axiosConfig';
import { useNavigate, Link } from 'react-router-dom';

interface UserInfo {
    username: string;
    email?: string;
}

const Dashboard: React.FC = () => {
    const [user, setUser] = useState<UserInfo | null>(null);
    const navigate = useNavigate();

    useEffect(() => {
        axiosInstance.get('/dashboard/userinfo')
            .then((response) => setUser(response.data))
            .catch((error) => console.error('Erro ao buscar dados protegidos:', error));
    }, []);

    const handleLogout = () => {
        localStorage.removeItem('token');
        navigate('/login');
    };

    return (
        <div className="min-h-screen bg-gray-50 dark:bg-gray-900">
            {/* Navbar */}
            <nav className="bg-white dark:bg-gray-800 shadow px-6 py-4 flex justify-between items-center">
                <div className="flex space-x-4">
                    <Link to="/" className="text-blue-600 font-semibold hover:underline">Home</Link>
                    <Link to="/api/trips" className="text-blue-600 font-semibold hover:underline">Trips API</Link>
                    <Link to="/api/users" className="text-blue-600 font-semibold hover:underline">Users API</Link>
                    <Link to="/api/vehicles" className="text-blue-600 font-semibold hover:underline">Vehicles API</Link>
                </div>
                <div className="flex items-center space-x-4">
                    {user && (
                        <span className="text-gray-700 dark:text-gray-300">
                            Olá, <strong>{user.Username}</strong>
                        </span>
                    )}
                    <button
                        onClick={handleLogout}
                        className="bg-red-600 text-white px-3 py-1 rounded hover:bg-red-700"
                    >
                        Logout
                    </button>
                </div>
            </nav>

            {/* Conteúdo principal */}
            <main className="p-6 flex flex-col items-center justify-center text-center">
                <h1 className="text-3xl font-bold text-gray-800 dark:text-white mb-4">Dashboard do Projeto</h1>
                <p className="text-lg text-gray-600 dark:text-gray-300 max-w-xl">
                    Esta é a interface principal do projeto onde podes navegar pelas APIs, ver dados protegidos e interagir com as funcionalidades do sistema.
                </p>

                {/* Dados do utilizador */}
                {user && (
                    <div className="mt-6 bg-gray-100 dark:bg-gray-800 p-4 rounded shadow w-full max-w-2xl">
                        <h2 className="text-lg font-semibold text-gray-700 dark:text-white mb-2">Informação do Utilizador</h2>
                        <pre className="bg-gray-800 text-white p-4 rounded-lg text-left">
                            {JSON.stringify(user, null, 2)}
                        </pre>
                    </div>
                )}
            </main>
        </div>
    );
};

export default Dashboard;
