import React from 'react';

const Footer: React.FC = () => {
    return (
        <footer className="bg-gray-800 text-white py-4 mt-8">
            <div className="max-w-7xl mx-auto text-center">
                <p>&copy; 2025 Hopmate. All rights reserved.</p>
                <div className="mt-2">
                    <a href="#" className="text-blue-400 hover:text-blue-600">Privacy Policy</a> |
                    <a href="#" className="text-blue-400 hover:text-blue-600"> Terms of Service</a>
                </div>
            </div>
        </footer>
    );
};

export default Footer;
