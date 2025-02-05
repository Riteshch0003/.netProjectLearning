import React, { useState } from 'react';
import { Link } from 'react-router-dom';

const Navbar = () => {
    // Simulating login state
    const [isLoggedIn, setIsLoggedIn] = useState(false); // Set to true if user is logged in

    return (
      <nav className="bg-gradient-to-r from-indigo-600 via-purple-600 to-pink-600 text-white p-4 shadow-lg">
        <div className="max-w-screen-xl mx-auto flex justify-between items-center">
          {/* App Name/Logo */}
          <div className="text-3xl font-extrabold">
            <Link to="/" className="hover:text-indigo-300 transition duration-300">MyApp</Link>
          </div>

          {/* Navbar Links */}
          <ul className="flex space-x-8">
            {/* Always show these links */}
            <li>
              <Link to="/" className="text-xl hover:text-indigo-300 transition duration-300 font-medium">Home</Link>
            </li>

            {/* Show login if not logged in */}
            {!isLoggedIn ? (
              <>
                <li>
                  <Link to="/login" className="text-xl hover:text-indigo-300 transition duration-300 font-medium">Login</Link>
                </li>
              </>
            ) : (
              // Show these links after login
              <>
                <li>
                  <Link to="/createpost" className="text-xl hover:text-indigo-300 transition duration-300 font-medium">Create Post</Link>
                </li>
                <li>
                  <Link to="/postlist" className="text-xl hover:text-indigo-300 transition duration-300 font-medium">Post List</Link>
                </li>
              </>
            )}
          </ul>

          {/* Logout Button */}
          <div>
            {isLoggedIn && (
              <button 
                onClick={() => setIsLoggedIn(false)} 
                className="bg-red-600 hover:bg-red-700 px-6 py-3 rounded-lg text-white font-semibold transition duration-300">
                Logout
              </button>
            )}
          </div>
        </div>
      </nav>
    );
};

export default Navbar;
