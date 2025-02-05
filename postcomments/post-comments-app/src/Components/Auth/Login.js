import React, { useState } from 'react';
import axios from 'axios';
import { useNavigate } from 'react-router-dom';

const Login = () => {
    const [email, setEmail] = useState('');
    const [password, setPassword] = useState('');
    const [error, setError] = useState(null);
    const navigate = useNavigate();

    const handleSubmit = async (e) => {
        e.preventDefault();
    
        try {
            // Send login request to the backend API
            const response = await axios.post('http://localhost:5041/api/PostComments/login', {
                email,
                password,
            });
    
            // On success, store the JWT token and userId in localStorage
            if (response.status === 200) {
                localStorage.setItem('token', response.data.token); 
                localStorage.setItem('userId', response.data.userId); 
                navigate('/postlist'); 
            }
        } catch (err) {
            // Handle login error (invalid credentials, etc.)
            if (err.response && err.response.status === 401) {
                setError('Invalid email or password.');
            } else {
                setError('An unexpected error occurred.');
            }
        }
    };

    return (
        <div className="flex justify-center items-center min-h-screen bg-gradient-to-r from-indigo-500 via-purple-500 to-pink-500">
            <div className="bg-white p-8 rounded-xl shadow-2xl w-full max-w-lg">
                <h2 className="text-4xl font-extrabold text-center text-gray-800 mb-6">Login</h2>
                
                {/* Login Form */}
                <form onSubmit={handleSubmit}>
                    {/* Email Input */}
                    <div className="mb-6">
                        <label htmlFor="email" className="block text-lg font-medium text-gray-600">Email</label>
                        <input 
                            type="email" 
                            id="email" 
                            value={email} 
                            onChange={(e) => setEmail(e.target.value)} 
                            required
                            className="w-full px-6 py-4 mt-2 border border-gray-300 rounded-lg focus:outline-none focus:ring-4 focus:ring-indigo-500 focus:border-indigo-500 transition duration-300"
                        />
                    </div>

                    {/* Password Input */}
                    <div className="mb-6">
                        <label htmlFor="password" className="block text-lg font-medium text-gray-600">Password</label>
                        <input 
                            type="password" 
                            id="password" 
                            value={password} 
                            onChange={(e) => setPassword(e.target.value)} 
                            required
                            className="w-full px-6 py-4 mt-2 border border-gray-300 rounded-lg focus:outline-none focus:ring-4 focus:ring-indigo-500 focus:border-indigo-500 transition duration-300"
                        />
                    </div>

                    {/* Error Message */}
                    {error && <div className="text-red-600 text-lg text-center mb-4">{error}</div>}

                    {/* Submit Button */}
                    <button 
                        type="submit" 
                        className="w-full py-4 bg-indigo-600 text-white font-semibold text-xl rounded-lg hover:bg-indigo-700 transition duration-300"
                    >
                        Login
                    </button>
                </form>

                {/* Register Link */}
                <div className="mt-6 text-center">
                    <span className="text-lg text-gray-700">Don't have an account? </span>
                    <a 
                        href="/register" 
                        className="text-indigo-600 text-lg font-semibold hover:underline transition duration-300"
                    >
                        Register
                    </a>
                </div>
            </div>
        </div>
    );
};

export default Login;
