import React from 'react';
import { BrowserRouter as Router, Route, Routes } from 'react-router-dom';
import Login from './pages/Login';
import Register from './pages/Register';
import Dashboard from './pages/Dashboard';
import PrivateRoute from './components/PrivateRoute';
import CancelTripPage from './pages/Trip/CancelTripPage';
import TripList from './pages/Trip/TripList';
import TripCreate from './pages/Trip/TripCreate';

const App: React.FC = () => {
    return (
        <Router>
            <Routes>
                <Route path="/login" element={<Login />} />
                <Route path="/register" element={<Register />} />
                <Route
                    path="/dashboard"
                    element={
                        <PrivateRoute>
                            <Dashboard />
                        </PrivateRoute>
                    }
                />
                <Route path="/" element={<Login />} />
                <Route path="/trip/cancel/:id" element={<CancelTripPage />} />
                <Route path="/trips" element={<TripList />} />
                <Route path="/trip/create" element={<TripCreate />} />
            </Routes>
        </Router>
    );
};

export default App;