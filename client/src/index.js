import React from 'react';
import ReactDOM from 'react-dom';
import { BrowserRouter, Routes, Route } from 'react-router-dom';
import './index.css';
import App from './App';
import Confirm from './Confirm';
import Cancel from './Cancel';
import Download from './Download';
import reportWebVitals from './reportWebVitals';

ReactDOM.render(
  <React.StrictMode>
    <link rel='preconnect' href='https://fonts.googleapis.com'/>
    <link rel='preconnect' href='https://fonts.gstatic.com' crossOrigin='true'/>
    <link href='https://fonts.googleapis.com/css2?family=Open+Sans:wght@300;400;700;800&display=swap' rel='stylesheet'></link>
    <BrowserRouter>
      <Routes>
        <Route path='/' element={<App/>}/>
        <Route path='confirm/:id' element={<Confirm/>}/>
        <Route path='cancel/:id' element={<Cancel/>}/>
        <Route path='download/:id' element={<Download/>}/>
      </Routes>
    </BrowserRouter>
  </React.StrictMode>,
  document.getElementById('root')
);

// If you want to start measuring performance in your app, pass a function
// to log results (for example: reportWebVitals(console.log))
// or send to an analytics endpoint. Learn more: https://bit.ly/CRA-vitals
reportWebVitals();
