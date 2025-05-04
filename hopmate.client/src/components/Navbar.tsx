import React from 'react';
import { Link } from 'react-router-dom';

const Navbar: React.FC = () => {
    return (
        <nav className="bg-gray-800 p-4">
            <div className="flex justify-between items-center max-w-7xl mx-auto">
                <div className="text-white text-2xl">
                    <Link to="/">Hopmate</Link>
                </div>
            </div>
        </nav>
    );
};

export default Navbar;