import React, { useState } from 'react';
import axios from 'axios';

const Register = () => {
    const [username, setUsername] = useState('');
    const [email, setEmail] = useState('');
    const [password, setPassword] = useState('');
    const [message, setMessage] = useState('');
    const [error, setError] = useState('');

    const handleRegister = async (event) => {
        event.preventDefault();
        setMessage('');
        setError('');

        if (!username || !email || !password) {
            setError("All fields are required.");
            return;
        }

        try {
            const response = await axios.post('http://localhost:5041/api/PostComments/register', {
                username,
                email,
                password
            });

            setMessage(response.data.message); // Success message
            setUsername('');
            setEmail('');
            setPassword('');
        } catch (error) {
            if (error.response && error.response.data) {
                setError(error.response.data.message);
            } else {
                setError("An error occurred. Please try again.");
            }
        }
    };

    return (
        <div style={{ maxWidth: '400px', margin: 'auto', padding: '1em' }}>
            <h2>Register</h2>
            <form onSubmit={handleRegister}>
                <div style={{ marginBottom: '1em' }}>
                    <label>Username</label>
                    <input
                        type="text"
                        value={username}
                        onChange={(e) => setUsername(e.target.value)}
                        required
                        style={{ width: '100%', padding: '0.5em', marginTop: '0.5em' }}
                    />
                </div>
                <div style={{ marginBottom: '1em' }}>
                    <label>Email</label>
                    <input
                        type="email"
                        value={email}
                        onChange={(e) => setEmail(e.target.value)}
                        required
                        style={{ width: '100%', padding: '0.5em', marginTop: '0.5em' }}
                    />
                </div>
                <div style={{ marginBottom: '1em' }}>
                    <label>Password</label>
                    <input
                        type="password"
                        value={password}
                        onChange={(e) => setPassword(e.target.value)}
                        required
                        style={{ width: '100%', padding: '0.5em', marginTop: '0.5em' }}
                    />
                </div>
                {error && <p style={{ color: 'red' }}>{error}</p>}
                {message && <p style={{ color: 'green' }}>{message}</p>}
                <button type="submit" style={{ padding: '0.5em', width: '100%' }}>Register</button>
            </form>
        </div>
    );
};

export default Register;
