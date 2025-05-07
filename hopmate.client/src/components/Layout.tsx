/* -*- coding: utf-8 -*- */
import React, { useEffect, useState } from 'react';
import { Link, useNavigate } from 'react-router-dom';
import { axiosInstance } from '../axiosConfig';

interface Props {
    children: React.ReactNode;
}

interface UserInfo {
    Username: string;
}

const Layout: React.FC<Props> = ({ children }) => {
    const [user, setUser] = useState<UserInfo | null>(null);
    const navigate = useNavigate();

    useEffect(() => {
        axiosInstance.get('/dashboard/userinfo')
            .then((res) => setUser(res.data))
            .catch((err) => {
                console.error('Erro ao buscar utilizador:', err);
                setUser(null); // opcional
            });
    }, []);

    const handleLogout = () => {
        localStorage.removeItem('token');
        localStorage.removeItem('userId');
        navigate('/login');
    };

    return (
        <div className="min-h-screen bg-gray-50 dark:bg-gray-900">
            {/* Navbar */}
            <nav className="bg-white dark:bg-gray-800 shadow px-6 py-4 flex justify-between items-center">
                <div className="flex space-x-4">
                    <Link to="/" className="text-blue-600 font-semibold hover:underline">Home</Link>
                    <Link to="/trips" className="text-blue-600 font-semibold hover:underline">Trips API</Link>
                    <Link to="/users" className="text-blue-600 font-semibold hover:underline">Users API</Link>
                    <Link to="/vehicles" className="text-blue-600 font-semibold hover:underline">Vehicles API</Link>
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
            <main>{children}</main>
        </div>
    );
};

export default Layout;
