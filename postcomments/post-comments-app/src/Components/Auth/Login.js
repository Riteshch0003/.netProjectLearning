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

            // On success, store the JWT token in localStorage
            if (response.status === 200) {
                localStorage.setItem('token', response.data.token);
                navigate('/my-posts'); // Redirect to posts page after login
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
        <div className="login-container">
            <h2>Login</h2>
            <form onSubmit={handleSubmit}>
                <div>
                    <label>Email</label>
                    <input 
                        type="email" 
                        value={email} 
                        onChange={(e) => setEmail(e.target.value)} 
                        required 
                    />
                </div>
                <div>
                    <label>Password</label>
                    <input 
                        type="password" 
                        value={password} 
                        onChange={(e) => setPassword(e.target.value)} 
                        required 
                    />
                </div>
                {error && <div style={{ color: 'red' }}>{error}</div>}
                <button type="submit">Login</button>
            </form>
        </div>
    );
};

export default Login;
