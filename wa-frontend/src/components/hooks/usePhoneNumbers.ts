import { getPhoneNumbers } from "@/services/chatService";
import { PhoneNumber } from "@/types/chat";
import { useEffect, useState } from "react";

interface PhoneNumberProps {
    connection: any
}

export const usePhoneNumber = ({
    connection
}: PhoneNumberProps) => {
    const [phoneNumbers, setPhoneNumbers] = useState<PhoneNumber[]>([]);
    const [totalUnread, setTotalUnread] = useState(0);

    const fetchPhoneNumbers = async () => {
        try {
            const phoneNumbers = await getPhoneNumbers();
            setPhoneNumbers(phoneNumbers.data);

        } catch (error) { console.error("Initial fetch failed", error); }
    };

    useEffect(() => {
        const totalUnread = phoneNumbers.reduce((acc, app) => acc + (app.unread_count || 0), 0);
        setTotalUnread(totalUnread);
    }, [phoneNumbers, fetchPhoneNumbers]);

    useEffect(() => {
        connection.on("UpdatePhoneNumbers", handleUpdatePhoneNumbers);

        return () => {
            connection.off("UpdatePhoneNumbers", handleUpdatePhoneNumbers);
        };
    }, [connection]);


    const handleUpdatePhoneNumbers = (conv: PhoneNumber[]) => {
        setPhoneNumbers((prevPhoneNumbers) => {
            // 1. Buat salinan dari state saat ini agar tidak mengubahnya langsung
            const updated = [...prevPhoneNumbers];

            conv.forEach((newPn) => {
                // 2. Cari apakah ID sudah ada di dalam list
                const index = updated.findIndex((pn) => pn.phone_number_id === newPn.phone_number_id);

                if (index !== -1) {
                    // 3. Jika ada, perbarui datanya
                    updated[index] = newPn;
                } else {
                    // 4. Jika tidak ada, tambahkan data baru ke paling belakang
                    updated.push(newPn);
                }
            });

            return updated;
        });

        const totalUnread = phoneNumbers.reduce((acc, app) => acc + (app.unread_count || 0), 0);
        setTotalUnread(totalUnread);
    }

    return {
        phoneNumbers,
        fetchPhoneNumbers,
        totalUnread
    }
}