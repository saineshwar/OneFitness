"use client";

import { useEffect, useRef, useState, type ChangeEvent } from "react";
import { Icon } from "@/components/icons";

// Photos are center-cropped to a square and re-encoded as JPEG, which keeps each one around 20–40 KB.
const PHOTO_SIZE = 320;
const JPEG_QUALITY = 0.85;

type PhotoCaptureProps = {
  value: string | null;
  onChange: (photo: string | null) => void;
  loading?: boolean;
};

function toSquareJpeg(source: CanvasImageSource, width: number, height: number) {
  const side = Math.min(width, height);
  const canvas = document.createElement("canvas");
  canvas.width = PHOTO_SIZE;
  canvas.height = PHOTO_SIZE;
  canvas.getContext("2d")!.drawImage(source, (width - side) / 2, (height - side) / 2, side, side, 0, 0, PHOTO_SIZE, PHOTO_SIZE);
  return canvas.toDataURL("image/jpeg", JPEG_QUALITY);
}

export function PhotoCapture({ value, onChange, loading }: PhotoCaptureProps) {
  const videoRef = useRef<HTMLVideoElement>(null);
  const fileInputRef = useRef<HTMLInputElement>(null);
  const streamRef = useRef<MediaStream | null>(null);
  const [cameraOn, setCameraOn] = useState(false);
  const [starting, setStarting] = useState(false);
  const [error, setError] = useState<string | null>(null);

  function stopCamera() {
    streamRef.current?.getTracks().forEach((track) => track.stop());
    streamRef.current = null;
    setCameraOn(false);
  }

  // Release the webcam when the modal closes.
  useEffect(() => stopCamera, []);

  async function startCamera() {
    setError(null);
    if (!navigator.mediaDevices?.getUserMedia) {
      setError("Camera is not available in this browser. The page must be opened over HTTPS or localhost.");
      return;
    }
    setStarting(true);
    try {
      const stream = await navigator.mediaDevices.getUserMedia({
        video: { facingMode: "user", width: { ideal: 640 }, height: { ideal: 480 } },
        audio: false,
      });
      streamRef.current = stream;
      setCameraOn(true);
    } catch (err) {
      const name = err instanceof DOMException ? err.name : "";
      setError(
        name === "NotAllowedError"
          ? "Camera permission was denied. Allow camera access in the browser to take a photo."
          : name === "NotFoundError"
            ? "No camera was found on this device."
            : "Could not start the camera.",
      );
    } finally {
      setStarting(false);
    }
  }

  // Attach the stream once the <video> element is rendered.
  useEffect(() => {
    if (cameraOn && videoRef.current && streamRef.current) {
      videoRef.current.srcObject = streamRef.current;
    }
  }, [cameraOn]);

  function capture() {
    const video = videoRef.current;
    if (!video || !video.videoWidth) return;
    onChange(toSquareJpeg(video, video.videoWidth, video.videoHeight));
    stopCamera();
  }

  function handleFile(event: ChangeEvent<HTMLInputElement>) {
    const file = event.target.files?.[0];
    event.target.value = "";
    if (!file) return;
    if (!file.type.startsWith("image/")) {
      setError("Please choose an image file.");
      return;
    }
    setError(null);
    const url = URL.createObjectURL(file);
    const image = new Image();
    image.onload = () => {
      onChange(toSquareJpeg(image, image.naturalWidth, image.naturalHeight));
      URL.revokeObjectURL(url);
    };
    image.onerror = () => {
      setError("Could not read that image.");
      URL.revokeObjectURL(url);
    };
    image.src = url;
  }

  const secondaryButton =
    "inline-flex items-center gap-1.5 rounded-lg border border-slate-300 px-2.5 py-1.5 text-xs font-medium text-slate-600 transition hover:bg-slate-50 disabled:cursor-not-allowed disabled:opacity-60 dark:border-slate-700 dark:text-slate-300 dark:hover:bg-slate-800";

  return (
    <div className="flex items-start gap-4">
      <div className="relative h-28 w-28 shrink-0 overflow-hidden rounded-xl border border-slate-200 bg-slate-50 dark:border-slate-700 dark:bg-slate-800">
        {cameraOn ? (
          <video ref={videoRef} autoPlay playsInline muted className="h-full w-full -scale-x-100 object-cover" />
        ) : value ? (
          // eslint-disable-next-line @next/next/no-img-element -- data URL, nothing for next/image to optimize
          <img src={value} alt="Member photo" className="h-full w-full object-cover" />
        ) : (
          <div className="flex h-full w-full items-center justify-center text-slate-300 dark:text-slate-600">
            {loading ? (
              <span className="h-5 w-5 animate-spin rounded-full border-2 border-slate-300 border-t-indigo-500" />
            ) : (
              <Icon name="user" className="h-12 w-12" />
            )}
          </div>
        )}
      </div>

      <div className="space-y-2">
        <p className="text-sm font-medium text-slate-700 dark:text-slate-300">
          Photo <span className="font-normal text-slate-400">(optional)</span>
        </p>
        <div className="flex flex-wrap gap-2">
          {cameraOn ? (
            <>
              <button
                type="button"
                onClick={capture}
                className="inline-flex items-center gap-1.5 rounded-lg bg-indigo-600 px-2.5 py-1.5 text-xs font-semibold text-white shadow-sm transition hover:bg-indigo-500"
              >
                Capture
              </button>
              <button type="button" onClick={stopCamera} className={secondaryButton}>
                Cancel
              </button>
            </>
          ) : (
            <>
              <button type="button" onClick={startCamera} disabled={starting || loading} className={secondaryButton}>
                {starting ? "Starting camera…" : value ? "Retake Photo" : "Take Photo"}
              </button>
              <button
                type="button"
                onClick={() => fileInputRef.current?.click()}
                disabled={loading}
                className={secondaryButton}
              >
                Upload
              </button>
              {value && (
                <button
                  type="button"
                  onClick={() => onChange(null)}
                  className="inline-flex items-center gap-1.5 rounded-lg px-2.5 py-1.5 text-xs font-medium text-red-600 transition hover:bg-red-50 dark:text-red-400 dark:hover:bg-red-950"
                >
                  <Icon name="trash" className="h-3.5 w-3.5" />
                  Remove
                </button>
              )}
            </>
          )}
        </div>
        <input ref={fileInputRef} type="file" accept="image/*" onChange={handleFile} className="hidden" />
        {error && <p className="max-w-xs text-xs text-red-600 dark:text-red-400">{error}</p>}
      </div>
    </div>
  );
}
