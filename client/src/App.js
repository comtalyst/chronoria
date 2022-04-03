import React, {useState} from 'react';
import logo from './media/logo.png';
import './App.css';
import 'flowbite';
import _ from 'lodash';
import axios from 'axios';

function App() {
  const [selfSend, setSelfSend] = useState(false);
  const [senderEmailRaw, setSenderEmailRaw] = useState('');
  const [senderNameRaw, setSenderNameRaw] = useState('');
  const [recipientEmailRaw, setRecipientEmailRaw] = useState('');
  const [recipientNameRaw, setRecipientNameRaw] = useState('');
  const [textRaw, setTextRaw] = useState('');
  const [fileRaw, setFileRaw] = useState(null);
  const [sendTimeRaw, setSendTimeRaw] = useState(null);
  
  const [submitting, setSubmitting] = useState(false);
  const [currentWarning, setCurrentWarning] = useState('');
  const [currentError, setCurrentError] = useState('');

  const trueSubmit = async (e) => {
    e.preventDefault();
    const senderEmail = senderEmailRaw.trim();
    const senderName = senderNameRaw.trim();
    const [recipientEmail, recipientName] = (() => {
      if(selfSend){
        return [senderEmail, senderName];
      }
      else{
        return [recipientEmailRaw.trim(), recipientNameRaw.trim()];
      }
    })();
    const text = textRaw.trim();
    const sendTime = Date.parse(sendTimeRaw);
    if(sendTime < Date.now()){
      setCurrentWarning('Sorry, but we cannot send a letter to the past at this time (but we wish we could ;-; )');
      return false;
    }
    else if(sendTime < Date.now() + 172800000){
      setCurrentWarning('Destination time needs to be at least 2 days from now');
      return false;
    }
    if(fileRaw == null){
      // construct request body as json
      const reqBody = {
        senderEmail, senderName, recipientEmail, recipientName, sendTime, text
      }
      await axios.post('https://localhost:7056/api/submit/text', reqBody);
    }
    else{
      if(fileRaw.size > 200000000){
        throw new Error('File size must not exceed 200 MB');
      }
      const reqBody = {
        senderEmail, senderName, recipientEmail, recipientName, sendTime, text
      }
      reqBody.textLocation = 'Before';              // not implementing this for now
      // construct request body as form (for file)
      const formData = new FormData();
      for (const [key, val] of Object.entries(reqBody)){
        formData.append(key, val);
      }
      formData.append('FormFile', fileRaw);
      formData.append('FileName', fileRaw.name);
      await axios.post('https://localhost:7056/api/submit/file', formData);
    }
    return true;
  }
  const submit = async (e) => {
    e.preventDefault();
    setCurrentError('');
    setCurrentWarning('');
    setSubmitting(true);
    try{
      const res = await trueSubmit(e);
      if(!res){
        setSubmitting(false);
        return false;
      }
    } catch (e){
      setCurrentError(e.message);
      setSubmitting(false);
      return false;
    }
    // TODO: do something
    return true;
  }
  
  return (
    <div className='flex flex-col min-h-screen bg-light_bg text-light_text text-lg'>
      <link rel="preconnect" href="https://fonts.googleapis.com"/>
      <link rel="preconnect" href="https://fonts.gstatic.com" crossOrigin='true'/>
      <link href="https://fonts.googleapis.com/css2?family=Open+Sans:wght@300;400;700;800&display=swap" rel="stylesheet"></link>

      <div className='flex flex-wrap align-middle justify-around min-h-screen'>
        <div className='flex-col px-10 pt-12 w-fit'>
          <div className='mb-20'>
            <div className='text-5xl sm:text-8xl'>
              <span className=' font-black leading-snug'>
                Time Travel <br/> with <br/>
                <span className='text-light_hl'>
                  Timelette!
                </span>
              </span>
            </div>
          </div>
          <div className='mb-20 max-w-[34rem]'>
            <div className='text-2xl sm:text-3xl'>
              <span className='font-light leading-snug'>
                A message, a hidden treasure map, a confession, a memory, or whatever it is.
                Send it as <span className='text-light_hl font-bold'>a letter</span> to yourself or someone else from the future.
              </span>
            </div>
          </div>
          <button className='bg-light_hl w-fit px-5 py-2 text-light_hl_subtext font-bold text-2xl rounded
                          hover:bg-light_hl_l hover:scale-105
                          transition-all duration-200'>
            Get Started
          </button>
        </div>
        <div className='flex flex-col px-20 justify-around'>
          <img src={logo} alt='Timelette' className='w-[48rem] h-fit hover:scale-95 transition-all duration-500'/>
        </div>
      </div>

      <div className='flex flex-col min-h-screen px-6 md:px-24 pt-12'>
        <div className='flex flex-col mb-8'>
          
          <form onSubmit={submit}>
            <div className='flex flex-wrap gap-y-4 justify-between mb-8'>
              <div className='flex flex-col min-w-[50%] grow space-y-3 px-4'>
                <label htmlFor='senderEmail'>
                  Sender's Email (yes, yours!)
                </label>
                <input id='senderEmail' type='email' placeholder='you@example.com' required maxLength='255' onChange={(e) => setSenderEmailRaw(e.target.value)}
                className='border border-gray-300 text-gray-900 rounded focus:ring-light_hl focus:border-light_hl'/>
              </div>
              <div className='flex flex-col min-w-[50%] grow space-y-3 px-4'>
                <label htmlFor='senderName'>
                  Sender's Alias
                </label>
                <input id='senderName' type='text' placeholder='Full name, nickname, or any identifier!' required maxLength='255' onChange={(e) => setSenderNameRaw(e.target.value)}
                className='border border-gray-300 text-gray-900 rounded focus:ring-light_hl focus:border-light_hl'/>
              </div>
            </div>
            
            <div className='flex mb-3 px-4 items-center'>
              <input id='selfSend' aria-describedby='selfSend' type='checkbox' onChange={() => setSelfSend(!selfSend)}
                className='w-4 h-4 rounded border border-gray-300 focus:ring-3 focus:ring-light_hl '/>
              <label htmlFor='selfSend' className='text-sm pl-3'>This letter is for myself</label>
            </div>

            {(!selfSend)? (
            <div className='flex flex-wrap gap-y-4 justify-between mb-8'>
              <div className='flex flex-col min-w-[50%] grow space-y-3 px-4'>
                <label htmlFor='recipientEmail'>
                  Recipient's Email &nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;
                </label>
                <input id='recipientEmail' type='email' placeholder='they@example.com' required maxLength='255' onChange={(e) => setRecipientEmailRaw(e.target.value)}
                  className='border border-gray-300 text-gray-900 rounded focus:ring-light_hl focus:border-light_hl'/>
              </div>
              <div className='flex flex-col min-w-[50%] grow space-y-3 px-4'>
                <label htmlFor='recipientName'>
                  Recipient's Alias
                </label>
                <input id='recipientName' type='text' placeholder='How would you call this individual?' required maxLength='255' onChange={(e) => setRecipientNameRaw(e.target.value)}
                  className='border border-gray-300 text-gray-900 rounded focus:ring-light_hl focus:border-light_hl'/>
              </div>
            </div>
            ):(<div/>)}

            <div className='flex flex-col mb-8 space-y-3 px-4'>
              <label htmlFor='text'>
                Inside the Envelope
              </label>
              <textarea id='text' rows='5' placeholder='Hello...? (max 10,000 characters)' required maxLength='10000' onChange={_.debounce((ev) => setTextRaw(ev.target.value), 200)}
                className='w-full border border-gray-300 text-gray-900 rounded focus:ring-light_hl focus:border-light_hl'/>
              <input className='text-sm text-gray-900 bg-white rounded border border-gray-300 cursor-pointer 
                focus:outline-none focus:border-transparent focus:ring-2 focus:ring-light_hl' id='fileUpload' type='file' onChange={(e) => setFileRaw(e.target.files[0])}/>
              <label htmlFor='fileUpload' className='text-sm text-gray-500'>File upload is optional. The maximum allowed file size is 200 MB. Recommended extensions: .pdf, .txt, .jpg, .mp4, .zip</label>
            </div>

            <div className='flex flex-col mb-8 space-y-3'>
              <div className='flex flex-col max-w-[18rem] space-y-3 px-4'>
                <label htmlFor='sendTimeRaw'>
                  Destination Time
                </label>
                <input type='datetime-local' className='text-gray-900 bg-white rounded border border-gray-300 focus:ring-light_hl focus:border-light_hl' 
                  placeholder='Select date' required onChange={(e) => setSendTimeRaw(e.target.value)}/>
              </div>
              <label htmlFor='sendTimeRaw' className='px-4 -mt-10 text-sm text-gray-500'>Date and time is based on your local machine time zone. It needs to be at least 2 days from now.</label>
            </div>

            { (currentError || currentWarning)? (<div/>) : (
                <div class="p-4 mx-4 mb-4 text-sm">
                  &nbsp;
                </div>
              )
            }
            { (currentWarning === '')? (<div/>) : (
                <div class="p-4 mx-4 mb-4 text-sm text-yellow-700 bg-yellow-100 rounded" role="alert">
                  <span class="font-semibold">Warning!</span> {currentWarning}
                </div>
              )
            }
            { (currentError === '')? (<div/>) : (
                <div class="p-4 mx-4 mb-4 text-sm text-red-700 bg-red-100 rounded" role="alert">
                  <span class="font-semibold">Error!</span> {currentError}
                </div>
              )
            }
            <div className='flex flex-col mb-8 space-y-3 px-4'>
              <span className='text-sm text-gray-500'>By clicking submit, you agree to the <a className='text-light_hl underline' href='https://google.com'>terms and conditions</a>.</span>
              {
                (!submitting)? (
                  <button className='bg-light_hl w-fit px-5 py-2 text-light_hl_subtext font-bold text-2xl rounded
                                 hover:bg-light_hl_l hover:scale-105
                                 transition-all duration-200'
                          type='submit'>
                    Submit
                  </button>
                ) : (
                  <button className='bg-gray-400 w-fit px-5 py-2 text-light_hl_subtext font-bold text-2xl rounded
                                 transition-all duration-200'
                          type='submit' disabled>
                    Processing...
                  </button>
                )
              }
            </div>
          </form>
          
        </div>
      </div>
    </div>
  );
}

export default App;
